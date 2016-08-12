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

        $scope.sendText = function (text) {
            if ($scope.correctPrompt) {
                showMessage(text, text);
            } else {
                showCandidates(text);
            }
        }

        function showMessage(text, correctText) {
            var commonWords = getCommonWords(text, correctText);
            text = renderDifference(text, commonWords, false);
            correctText = renderDifference(correctText, commonWords, true);
            var leftRecord = document.createElement("div");
            leftRecord.innerHTML = correctText;
            leftRecord.setAttribute("class", "record");
            document.getElementById("left-content").appendChild(leftRecord);
            document.getElementById("left-content").scrollIntoView();
            var rightRecord = document.createElement("div");
            rightRecord.innerHTML = text;
            rightRecord.setAttribute("class", "record");
            document.getElementById("right-content").appendChild(rightRecord);
            document.getElementById("right-content").scrollIntoView();
            $scope.correctPrompt = false;
            $scope.userInputs = "";
        }

        function getCommonWords(str1, str2) {
            if (!str1 || !str2)
                return null;
            var commonWords = [];
            var words = str1.replace("/[^a-zA-Z]/g", "").split(' ');
            var compareWords = str2.replace("/[^a-zA-Z]/g", "").split(' ');
            for (var i in words) {
                var word = words[i];
                if (compareWords.indexOf(word) >= 0) {
                    commonWords.push(word);
                }
            }
            return commonWords;
        }

        function renderDifference(text, commonWords, isCorrected) {
            if (!text)
                return text;
            var words = text.replace("/[^a-zA-Z]/g", "").split(' ');
            for (var i in words) {
                var word = words[i];
                if (commonWords.indexOf(word) < 0) {
                    if (isCorrected) {
                        words[i] = word.replace(word, "<span style='color:green'><strong>" + word + "</strong></span>");
                    } else {
                        words[i] = word.replace(word, "<span style='color:red'><strong>" + word + "</strong></span>");
                    }
                }
                // add link to word if the word is an entity
                if (isCorrected) {
                    var linkEntity = linkToBingEntity(word, words[i]);
                    console.log("lined entity", linkEntity);
                    //words[i] = linkEntity;
                }
            }
            var renderText = '';
            for (var i in words) {
                if (i === 0) {
                    renderText + words[i];
                } else {
                renderText += ' ' + words[i];
                }
            }
            return renderText;
        }

        function showCandidates(reqText) {
            Project.sendRequestToMemory(
            {
                request: reqText
            }).$promise.then(function (response) {
                $scope.correctPrompt = true;
                console.log(response.text);
                $scope.candidates = response.text.split('\t');
                if ($scope.candidates.indexOf(reqText) >= 0) {
                    showMessage(reqText, reqText);
                }
            });
        }

        $scope.correctSentence = function (correctText) {
            //var text = $scope.getBingEntities(correctText);
            //Project.recognizeBingEntitis(
            //{
            //    sentence: correctText
            //}).$promise.then(function (response) {
            //    showMessage(response.text);
            //});
            showMessage($scope.userInputs, correctText);
        }

        function linkToBingEntity(word, renderedWord) {
            Project.getBingEntities({
                query: word
            }).$promise.then(function (response) {
                var entities = response.entities;
                if (entities.length === 0) {
                    return word;
                }
                console.log("entities: ", entities);
                return "<a href='#'>" + renderedWord + "</a>";
            });
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

        $scope.keyPressOnTextarea = function (event, text) {
            if (event.which === 13) {
                $scope.sendText(text);
                event.preventDefault();
            }
        }

        $scope.keyPressOnDiv = function (event, text) {
            if (event.which === 13) {
                $scope.correctSentence(text);
                event.preventDefault();
            }
        }

    };
}(window));

// init angular controller
app.controller('ChattingController', ['$scope', 'Project', ChattingController]);