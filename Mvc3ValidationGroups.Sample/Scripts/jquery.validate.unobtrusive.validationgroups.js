$(function () {
    function onGroupedErrors(form, validator) {  // 'this' is the form element
        var containers = $(this).find("div.validation-summary, div[data-valmsg-summary=true]");
        var disabled = $(this).find("div:not[class~='validation-summary-disabled']");
        $.each(containers, function () {
            var container = $(this);
            var showmsgs = $(this).attr('data-valmsg-summary');
            list = container.find("ul");
            var groupName = container.attr("data-val-valgroup-name");
            if (list && list.length && validator.errorList.length) {
                list.empty();
                if (disabled.index(container) == -1) {
                    if (groupName) {
                        $.each(groupName.split(' '), function (index, value) {
                            $.each(validator.errorList, function () {
                                if ($("#" + $(this.element)[0].id + "[data-val-valgroup-name~='" + value + "']").length > 0) {
                                    var msg = this.message;
                                    var exists = false;
                                    $.each($('li', list), function () {
                                        if ($(this).text() == msg) {
                                            exists = true;
                                        }
                                    });
                                    if (!exists) {
                                        $("<li />").html(this.message).appendTo(list);
                                    }
                                }
                            });
                        });
                    }
                    else {
                        $.each(validator.errorList, function () {
                            var msg = this.message;
                            var exists = false;
                            $.each($('li', list), function () {
                                if ($(this).text() == msg) {
                                    exists = true;
                                }
                            });
                            if (!exists) {
                                $("<li />").html(this.message).appendTo(list);
                            }
                        });
                    }
                }
                if (list.children().length > 0) {
                    container.addClass("validation-summary-errors").removeClass("validation-summary-valid");
                    if (!showmsgs) {
                        list.empty();
                        $("<li style=\"display: none;\" />").html("").appendTo(list);
                    }
                }
                else {
                    container.addClass("validation-summary-valid").removeClass("validation-summary-errors");
                    $("<li style=\"display: none;\" />").html("").appendTo(list);
                }
            }
        });
    }
    $('form').each(function (i, form) {
        $form = $(form);
        var val = $form.data('validator');
        val.settings.invalidHandler = onGroupedErrors;
        $form.unbind("invalid-form.validate"); // remove old handler
        $form.bind("invalid-form.validate", onGroupedErrors);
    });
    $.validator.unobtrusive.rollbackValidation = function (formElement, elementsToValidate) {
        $("input", formElement).not(elementsToValidate).each(function () {
            $(this).rules("add", $(this).data("rulesBackup"));
        });
    }
    $.validator.unobtrusive.configureValidation = function (triggerElement) {
        var inputElement = $(triggerElement);
        var formElement = $(inputElement.parents("form")[0]);
        var groupNames = inputElement.attr("data-val-valgroup-name");
        $("div[data-valmsg-summary=true]", formElement).each(function () {
            $(this).removeClass("validation-summary-disabled");
        });
        if (groupNames === undefined || groupNames === "") return;
        var elementsToValidate = null;
        $.each(groupNames.split(' '), function (index, value) {
            if (elementsToValidate === null) {
                elementsToValidate = $("*[data-val-valgroup-name~='" + value + "']", formElement);
            } else {
                $.merge(elementsToValidate, $("*[data-val-valgroup-name~='" + value + "']", formElement));
            }
        });
        if (elementsToValidate === undefined || elementsToValidate.length === 0) return;
        $("div[data-valmsg-summary=true]", formElement).not(elementsToValidate).each(function () {
            $(this).addClass("validation-summary-disabled");
        });
        var restore = false;
        $("input", formElement).not(elementsToValidate).each(function () {
            var rules = $(this).rules("remove");
            $(this).data("rulesBackup", rules);
            $(formElement).find("[data-valmsg-for='" + $(this).attr("name") + "']").addClass("field-validation-valid");
            $(this).removeClass("input-validation-error");
            restore = true;
        });
        if (restore) {
            formElement.one('submit', function (eventObject) {
                $.validator.unobtrusive.rollbackValidation(formElement, elementsToValidate);
            });
        }
        return elementsToValidate;
    }


    $(function () {
        $("input[type=submit], .causesvalidation").click(function () {
            $.validator.unobtrusive.configureValidation(this);
        });
    });

});