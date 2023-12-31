"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.downloadBrowserWithProgressBar = downloadBrowserWithProgressBar;
exports.logPolitely = logPolitely;
var _fs = _interopRequireDefault(require("fs"));
var _os = _interopRequireDefault(require("os"));
var _path = _interopRequireDefault(require("path"));
var _child_process = _interopRequireDefault(require("child_process"));
var _fileUtils = require("../../utils/fileUtils");
var _debugLogger = require("../../common/debugLogger");
var _manualPromise = require("../../utils/manualPromise");
var _utilsBundle = require("../../utilsBundle");
var _ = require(".");
function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }
/**
 * Copyright 2017 Google Inc. All rights reserved.
 * Modifications copyright (c) Microsoft Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

async function downloadBrowserWithProgressBar(title, browserDirectory, executablePath, downloadURLs, downloadFileName, downloadConnectionTimeout) {
  if (await (0, _fileUtils.existsAsync)((0, _.browserDirectoryToMarkerFilePath)(browserDirectory))) {
    // Already downloaded.
    _debugLogger.debugLogger.log('install', `${title} is already downloaded.`);
    return false;
  }
  const zipPath = _path.default.join(_os.default.tmpdir(), downloadFileName);
  try {
    const retryCount = 3;
    for (let attempt = 1; attempt <= retryCount; ++attempt) {
      _debugLogger.debugLogger.log('install', `downloading ${title} - attempt #${attempt}`);
      const url = downloadURLs[(attempt - 1) % downloadURLs.length];
      logPolitely(`Downloading ${title}` + _utilsBundle.colors.dim(` from ${url}`));
      const {
        error
      } = await downloadBrowserWithProgressBarOutOfProcess(title, browserDirectory, url, zipPath, executablePath, downloadConnectionTimeout);
      if (!error) {
        _debugLogger.debugLogger.log('install', `SUCCESS installing ${title}`);
        break;
      }
      if (await (0, _fileUtils.existsAsync)(zipPath)) await _fs.default.promises.unlink(zipPath);
      if (await (0, _fileUtils.existsAsync)(browserDirectory)) await _fs.default.promises.rmdir(browserDirectory, {
        recursive: true
      });
      const errorMessage = (error === null || error === void 0 ? void 0 : error.message) || '';
      _debugLogger.debugLogger.log('install', `attempt #${attempt} - ERROR: ${errorMessage}`);
      if (attempt >= retryCount) throw error;
    }
  } catch (e) {
    _debugLogger.debugLogger.log('install', `FAILED installation ${title} with error: ${e}`);
    process.exitCode = 1;
    throw e;
  } finally {
    if (await (0, _fileUtils.existsAsync)(zipPath)) await _fs.default.promises.unlink(zipPath);
  }
  logPolitely(`${title} downloaded to ${browserDirectory}`);
  return true;
}

/**
 * Node.js has a bug where the process can exit with 0 code even though there was an uncaught exception.
 * Thats why we execute it in a separate process and check manually if the destination file exists.
 * https://github.com/microsoft/playwright/issues/17394
 */
function downloadBrowserWithProgressBarOutOfProcess(title, browserDirectory, url, zipPath, executablePath, downloadConnectionTimeout) {
  const cp = _child_process.default.fork(_path.default.join(__dirname, 'oopDownloadBrowserMain.js'), [title, browserDirectory, url, zipPath, executablePath || '', String(downloadConnectionTimeout)]);
  const promise = new _manualPromise.ManualPromise();
  cp.on('message', message => {
    if ((message === null || message === void 0 ? void 0 : message.method) === 'log') _debugLogger.debugLogger.log('install', message.params.message);
  });
  cp.on('exit', code => {
    if (code !== 0) {
      promise.resolve({
        error: new Error(`Download failure, code=${code}`)
      });
      return;
    }
    if (!_fs.default.existsSync((0, _.browserDirectoryToMarkerFilePath)(browserDirectory))) promise.resolve({
      error: new Error(`Download failure, ${(0, _.browserDirectoryToMarkerFilePath)(browserDirectory)} does not exist`)
    });else promise.resolve({
      error: null
    });
  });
  cp.on('error', error => {
    promise.resolve({
      error
    });
  });
  return promise;
}
function logPolitely(toBeLogged) {
  const logLevel = process.env.npm_config_loglevel;
  const logLevelDisplay = ['silent', 'error', 'warn'].indexOf(logLevel || '') > -1;
  if (!logLevelDisplay) console.log(toBeLogged); // eslint-disable-line no-console
}