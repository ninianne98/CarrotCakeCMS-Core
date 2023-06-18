/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
* Generated: [[TIMESTAMP]]
*/

//==================== rudimentary callback functions
function __carrotOnAjaxRequestBegin(xhr) {
	console.log("This is the __OnAjaxRequestBegin Callback");
}
function __carrotOnAjaxRequestSuccess(data, status, xhr) {
	console.log("This is the __OnAjaxRequestSuccess: " + data);
}
function __carrotOnAjaxRequestFailure(xhr, status, error) {
	alert("This is the __OnAjaxRequestFailure Callback:" + error + "\r\n------------------\r\n" + xhr.responseText);
	console.log(error);
	console.log(xhr.responseText);
}
function __carrotOnAjaxRequestComplete(xhr, status) {
	console.log("This is the __OnAjaxRequestComplete Callback: " + status);
}

function __carrotAjaxPostForm(formid, div, postUri, replace) {
	var data = $("#" + formid).serialize();
	//console.log(formid + ' ' + div + " ---------------------- ");
	//console.log(data);

	$.ajax({
		type: 'POST',
		url: postUri,
		contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
		data: data,
		success: function (result, status, xhr) {
			//console.log(result);
			if (replace == true) {
				$("#" + div).html(result);
			}
		},
		error: function (xhr, status, error) {
			console.log('Failed __carrotAjaxPostForm');
			__carrotOnAjaxRequestFailure(xhr, status, error);
		}
	})
}

function __carrotAjaxPostFormData(data, div, postUri, replace) {
	//console.log(div + " ---------------------- ");
	//console.log(data);

	$.ajax({
		type: 'POST',
		url: postUri,
		contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
		data: data,
		success: function (result, status, xhr) {
			//console.log(result);
			if (replace == true) {
				$("#" + div).html(result);
			}
		},
		error: function (xhr, status, error) {
			console.log('Failed __carrotAjaxPostForm');
			__carrotOnAjaxRequestFailure(xhr, status, error);
		}
	})
}

function __OnAjaxRequestBegin(xhr) {
	__carrotOnAjaxRequestBegin(xhr);
}
function __OnAjaxRequestSuccess(data, status, xhr) {
	__carrotOnAjaxRequestSuccess(data, status, xhr);
}
function __OnAjaxRequestFailure(xhr, status, error) {
	__carrotOnAjaxRequestFailure(xhr, status, error);
}
function __OnAjaxRequestComplete(xhr, status) {
	__carrotOnAjaxRequestComplete(xhr, status);
}

//==================== dateTime stuff
function __carrotGetTimeFormat() {
	return "[[SHORTTIMEPATTERN]]";
}

function __carrotGetDateFormat() {
	return "[[SHORTDATEPATTERN]]";
}

function __carrotGetAMDateFormat() {
	return "[[AM_TIMEPATTERN]]";
}

function __carrotGetPMDateFormat() {
	return "[[PM_TIMEPATTERN]]";
}

function __carrotGetDateTemplate() {
	return "[[SHORTDATEFORMATPATTERN]]";
}

function __carrotGetDateTimeTemplate() {
	return "[[SHORTDATETIMEFORMATPATTERN]]";
}

//================================================================