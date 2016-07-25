/* ========================================================================
 * @description
 *
 * Chatting controller
 *
 * @dependency: restDataService
 * ======================================================================== */

(function (window) {
    'use strict';

    window.ChattingController = window.ChattingController || function ($scope, Project) {
        $scope.correctPrompt = false;
        $scope.candidates = [];
        $scope.userInputs = "";

        $scope.generateCandidates = function (reqText) {
            console.log(reqText);
            Project.sendRequestToMemory(
            {
                request: reqText
            }).$promise.then(function (response) {
                $scope.correctPrompt = true;
                console.log(response.text);
                $scope.candidates = response.text.split('\t');
            });
        }

        $scope.correctSentence = function (correctText) {
            $scope.userInputs = correctText;
            $scope.correctPrompt = false;
            var record = document.createElement("div");
            record.textContent = correctText;
            record.setAttribute("class", "record");
            document.getElementById("content").appendChild(record);

        }

        function getSentenceToBeCorrected($input, value) {
            var paramLeftPos = value.indexOf("(");
            var caretPos = getCaretPosition($input[0]);
            if (caretPos > paramLeftPos && caretPos < value.length) {
                var caretLeftStr = value.substring(paramLeftPos + 1, caretPos);
                var query = caretLeftStr.substring(caretLeftStr.lastIndexOf(",") + 1, caretPos).trim();
                if (query.startsWith("\"") || query.startsWith("\'")) {
                    return value;
                } else {
                    return query;
                }
            } else {
                return value;
            }
        }

        function getCaretPosition(input) {
            if (!input) {
                return;
            }
            if (input.selectionStart) {
                return input.selectionStart;
            }
            if (document.selection) {
                input.focus();
                var sel = document.selection.createRange(),
                    selLen = document.selection.createRange().text.length;
                sel.moveStart('character', -input.value.length);
                return sel.text.length - selLen;
            }
        }

    };
}(window));

// init angular controller
app.controller('ChattingController', ['$scope', 'Project', ChattingController]);