"use strict";

var _fs = _interopRequireDefault(require("fs"));
var _utilsBundle = require("../../utilsBundle");
var _network = require("../../utils/network");
var _manualPromise = require("../../utils/manualPromise");
var _zipBundle = require("../../zipBundle");
var _userAgent = require("../../utils/userAgent");
var _ = require(".");
function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }
/**
 * Copyright (c) Microsoft Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

function downloadFile(url, destinationPath, options) {
  const {
    progressCallback,
    log = () => {}
  } = options;
  log(`running download:`);
  log(`-- from url: ${url}`);
  log(`-- to location: ${destinationPath}`);
  let downloadedBytes = 0;
  let totalBytes = 0;
  const promise = new _manualPromise.ManualPromise();
  (0, _network.httpRequest)({
    url,
    headers: {
      'User-Agent': options.userAgent
    },
    timeout: options.connectionTimeout
  }, response => {
    log(`-- response status code: ${response.statusCode}`);
    if (response.statusCode !== 200) {
      let content = '';
      const handleError = () => {
        const error = new Error(`Download failed: server returned code ${response.statusCode} body '${content}'. URL: ${url}`);
        // consume response data to free up memory
        response.resume();
        promise.reject(error);
      };
      response.on('data', chunk => content += chunk).on('end', handleError).on('error', handleError);
      return;
    }
    totalBytes = parseInt(response.headers['content-length'] || '0', 10);
    log(`-- total bytes: ${totalBytes}`);
    const file = _fs.default.createWriteStream(destinationPath);
    file.on('finish', () => {
      if (downloadedBytes !== totalBytes) {
        log(`-- download failed, size mismatch: ${downloadedBytes} != ${totalBytes}`);
        promise.reject(new Error(`Download failed: size mismatch, file size: ${downloadedBytes}, expected size: ${totalBytes} URL: ${url}`));
      } else {
        log(`-- download complete, size: ${downloadedBytes}`);
        promise.resolve();
      }
    });
    file.on('error', error => promise.reject(error));
    response.pipe(file);
    response.on('data', onData);
  }, error => promise.reject(error));
  return promise;
  function onData(chunk) {
    downloadedBytes += chunk.length;
    progressCallback(downloadedBytes, totalBytes);
  }
}
function getDownloadProgress() {
  if (process.stdout.isTTY) return getAnimatedDownloadProgress();
  return getBasicDownloadProgress();
}
function getAnimatedDownloadProgress() {
  let progressBar;
  let lastDownloadedBytes = 0;
  return (downloadedBytes, totalBytes) => {
    if (!progressBar) {
      progressBar = new _utilsBundle.progress(`${toMegabytes(totalBytes)} [:bar] :percent :etas`, {
        complete: '=',
        incomplete: ' ',
        width: 20,
        total: totalBytes
      });
    }
    const delta = downloadedBytes - lastDownloadedBytes;
    lastDownloadedBytes = downloadedBytes;
    progressBar.tick(delta);
  };
}
function getBasicDownloadProgress() {
  // eslint-disable-next-line no-console
  const totalRows = 10;
  const stepWidth = 8;
  let lastRow = -1;
  return (downloadedBytes, totalBytes) => {
    const percentage = downloadedBytes / totalBytes;
    const row = Math.floor(totalRows * percentage);
    if (row > lastRow) {
      lastRow = row;
      const percentageString = String(percentage * 100 | 0).padStart(3);
      // eslint-disable-next-line no-console
      console.log(`|${'■'.repeat(row * stepWidth)}${' '.repeat((totalRows - row) * stepWidth)}| ${percentageString}% of ${toMegabytes(totalBytes)}`);
    }
  };
}
function toMegabytes(bytes) {
  const mb = bytes / 1024 / 1024;
  return `${Math.round(mb * 10) / 10} Mb`;
}
async function main() {
  const log = message => {
    var _process$send, _process;
    return (_process$send = (_process = process).send) === null || _process$send === void 0 ? void 0 : _process$send.call(_process, {
      method: 'log',
      params: {
        message
      }
    });
  };
  const [title, browserDirectory, url, zipPath, executablePath, downloadConnectionTimeout] = process.argv.slice(2);
  await downloadFile(url, zipPath, {
    progressCallback: getDownloadProgress(),
    userAgent: (0, _userAgent.getUserAgent)(),
    log,
    connectionTimeout: +downloadConnectionTimeout
  });
  log(`SUCCESS downloading ${title}`);
  log(`extracting archive`);
  log(`-- zip: ${zipPath}`);
  log(`-- location: ${browserDirectory}`);
  await (0, _zipBundle.extract)(zipPath, {
    dir: browserDirectory
  });
  if (executablePath) {
    log(`fixing permissions at ${executablePath}`);
    await _fs.default.promises.chmod(executablePath, 0o755);
  }
  await _fs.default.promises.writeFile((0, _.browserDirectoryToMarkerFilePath)(browserDirectory), '');
}
main().catch(error => {
  // eslint-disable-next-line no-console
  console.error(error);
  process.exit(1);
});