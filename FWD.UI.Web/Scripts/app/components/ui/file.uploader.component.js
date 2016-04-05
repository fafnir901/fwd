define([
	'jquery',
	'app/helpers/configurator.static.helper',
	'app/helpers/notification.static.helper',
	'app/helpers/appPath.static.helper'
], function($, conf, errorHelper, appPath) {

	var fileUploaderComponent = function() {
		var getViewModel = function(imageContainer, isNeedBigIcon) {
			if (this.viewModel == undefined) {
				var def = $.Deferred();
				var upload = function(file, param) {
					var xhr = new XMLHttpRequest();
					xhr.onload = xhr.onerror = function(e) {
						if (this.status != 200) {
							param.onError(this.responseText);
							return;
						}
						return param.onSuccess(e);
					};

					xhr.upload.onprogress = function(event) {
						param.onProgress(event.loaded, event.total, param.container);
					};
					xhr.open("POST", param.url, true);

					var formData = new FormData();
					formData.append("files", file);

					return xhr.send(formData);
				};
				this.viewModel = {};
				this.viewModel.upload = upload;
				this.viewModel.container = $('<div class="file_uploder_container"></div>');
				this.viewModel.wrapElement = $('<label class="file_uploader_wrapElement"></label>');
				this.viewModel.wrapElementBtn = $('<span class="file_uploader_wrapElementBtn">Выбрать</span>');
				this.viewModel.wrapElementText = $('<mark class="file_uploader_wrapElementText">Файл не выбран</mark>');
				this.viewModel.uploader = $('<input type="file" id="fileUploader" value="10000000"/>');
				this.viewModel.fileApi = (window.File && window.FileReader && window.FileList && window.Blob) ? true : false;

				this.viewModel.urlContainer = $('<div class="file_uploader_url_container"></div>');
				this.viewModel.urlLink = $('<input type="text" class="file_uploader_url_link" placeholder="Введите url-адрес картинки"/>');
				this.viewModel.uploadByUrlLinkButton = $('<span class="file_uploader_upload_by_url" role="button">Выбрать</span>');
				this.viewModel.urlContainer.append(this.viewModel.urlLink);
				this.viewModel.urlContainer.append(this.viewModel.uploadByUrlLinkButton);

				this.viewModel.progressContainer = $('<div class="progress_container"></div>');
				this.viewModel.progress = $('<div class="progress">0%</div>');

				this.viewModel.progressContainer.append(this.viewModel.progress);

				this.viewModel.imageList = [];
				this.viewModel.state = 'choose';
				var viewModel = this.viewModel;
				conf.uploadParams.container = viewModel.container;

				this.viewModel.uploader.off('change');
				this.viewModel.uploader.on('change', function() {
					var fileName = '';
					var inp = $(this);
					if (viewModel.fileApi && inp[0].files[0]) {
						fileName = inp[0].files[0].name;
					} else {
						fileName = inp.val().replace("C:\\fakepath\\", '');
					}

					if (!fileName.length) {
						return;
					}

					viewModel.wrapElementText.text(fileName);
					viewModel.wrapElementText.css('color', 'black');
					viewModel.wrapElementBtn.text('Загрузить');
					viewModel.state = 'load';

				}).change();

				this.viewModel.wrapElementBtn.off('click');

				var loadIcon = function(evt) {
					errorHelper.showInfo('Изображение загружено');
					var currentGuid = evt.srcElement.response.replace(/"/g, '');
					var img = $('<image image_id="' + currentGuid + '" class="img_icon" />');
					img.attr('src', appPath.getAppPath() + '/getTempImages/' + currentGuid + '/size-100x100');

					var container = $('#creation_image_container');
					container.append(img);

					$('.img_icon').off('mouseenter');
					$('.img_icon').off('mouseleave');

					$('.img_icon').on('mouseenter', function() {
						var that = $(this);
						var buttonDelete = $('<button class="delete_image" image_id="' + that.attr('image_id') + '"></button>');
						that.before(buttonDelete);
						buttonDelete.on('click', function() {
							var id = $(this).attr('image_id');
							$('.img_icon[image_id=' + id + ']').remove();
							$(this).remove();
						});
					});

					$('.img_icon').on('mouseleave', function() {
						var that = $(this);
						_.delay(function() {
							that.prev().remove();
						}, 1000);

					});
				};

				var loadBigIcon = function(evt, rootElement) {
					var img = $(rootElement.find('img')); //.remove();
					errorHelper.showInfo('Изображение загружено');
					var currentGuid = evt.srcElement.response.replace(/"/g, '');
					img.attr('image_id', currentGuid);
					img.animate({ opacity: '0' }, 200, function() {
						setTimeout(function() {
							$(this).attr('src', appPath.getAppPath() + '/getTempImages/' + currentGuid + '/size-300x400');
							$(this).animate({ opacity: '1' }, 200);
						}.bind(this), 100);

					});
				};

				this.viewModel.wrapElementBtn.on('click', function() {
					try {
						if (viewModel.state == 'choose') {

							viewModel.wrapElement.click();
							viewModel.progress.text('0%');
							viewModel.progress.css('width', '0px');
						} else {
							viewModel.state = 'choose';
							viewModel.wrapElementBtn.text('Выбрать');
							/*Вынести это наверх*/
							conf.uploadParams.onSuccess = function(evt) {
								if (!isNeedBigIcon) {
									loadIcon(evt);
								} else {
									loadBigIcon(evt, imageContainer);
								}
							};
							conf.uploadParams.onProgress = function (loaded, total) {
								if (viewModel.progressContainer.css('display') == 'none') {
									viewModel.progressContainer.css('display', 'block');
								}
								viewModel.progress.css('width', (loaded / total) * 100 + '%');
								viewModel.progress.text((loaded / total) * 100 + '%');
							};
							/**/
							viewModel.upload(viewModel.uploader[0].files[0], conf.uploadParams);
							viewModel.uploader[0].files[0] = null;
							viewModel.uploader.val(null);
							viewModel.wrapElementText.css('color', '');
							viewModel.wrapElementText.text('Файл не выбран');
						}
					} catch (e) {
						viewModel.state = 'choose';
						viewModel.uploader.val(null);
						viewModel.wrapElementText.css('color', '');
						viewModel.wrapElementText.text('Файл не выбран');
					}

				});

				this.viewModel.uploadByUrlLinkButton.off('click');
				this.viewModel.uploadByUrlLinkButton.on('click', function() {
					var link = $('.file_uploader_url_link');
					var errorClass = 'error_element';
					link.removeClass(errorClass);
					var errors = validateLink(link.val());
					if (errors.length > 0) {
						errorHelper.showError(errors.join('.'));
						link.addClass(errorClass);
					} else {
						var url = {
							url: link.val()
						};
						var obj = {
							additionalWidth: 50,
							additionalHeight: 50,
							additionalMinusLeft: 200
						};
						$.ajax({
							url: appPath.getAppPath() + '/uploadImageByLink',
							data: JSON.stringify(url),
							beforeSend: conf.beforeSend($('.file_uploder_container'), obj),
							type: 'POST',
							dataType: 'json',
							contentType: 'application/json'
						}).fail(function(data) {
							errorHelper.showError(data.responseText);
							obj = null;
						}).success(function(data) {
							conf.uploadParams.onSuccess = function (evt) {
								errorHelper.showInfo('Изображение загружено');
								var currentGuid = evt.replace(/"/g, '');
								var img = $('<image image_id="' + currentGuid + '" class="img_icon" />');
								img.attr('src', appPath.getAppPath() + '/getTempImages/' + currentGuid + '/size-100x100');

								var container = $('#creation_image_container');
								container.append(img);
								obj = null;
							};
							conf.uploadParams.onSuccess(data);
							link.val('');

							$('.img_icon').off('mouseenter');
							$('.img_icon').off('mouseleave');

							$('.img_icon').on('mouseenter', function() {
								var that = $(this);
								var buttonDelete = $('<button class="delete_image" image_id="' + that.attr('image_id') + '"></button>');
								that.before(buttonDelete);
								buttonDelete.on('click', function() {
									var id = $(this).attr('image_id');
									$('.img_icon[image_id=' + id + ']').remove();
									$(this).remove();
								});
							});

							$('.img_icon').on('mouseleave', function() {
								var that = $(this);
								_.delay(function() {
									that.prev().remove();
								}, 1000);

							});
						});
					}
				});

				this.viewModel.wrapElement.append(this.viewModel.wrapElementText);

				this.viewModel.wrapElement.append(this.viewModel.uploader); //<input type="hidden" name="MAX_FILE_SIZE" value="10000000"/>
				this.viewModel.wrapElement.append($('<input type="hidden" name="MAX_FILE_SIZE" value="10000000"/>'));
				this.viewModel.container.append(this.viewModel.wrapElement);
				this.viewModel.container.append(this.viewModel.wrapElementBtn);

				this.viewModel.container.append(this.viewModel.urlContainer);
				this.viewModel.container.append(this.viewModel.progressContainer);


			}
			return this.viewModel;
		}.bind(this);

		var validateLink = function(linkText) {
			var errors = [];
			if (linkText.length < 1) {
				errors.push('Ссылка не может быть пустой');
			}
			if (linkText.length < 10) {
				errors.push('Ссылка слишком коротка )');
			}

			var regex = new RegExp('^http|ftp|www.+[.]{1}[a-zA-Z]{2,6}$', 'gmi');
			if (!regex.test(linkText)) {
				errors.push('Что-то это не похоже на ссылку O_o');
			}
			return errors;
		};

		return {
			getUploader: function(imageContainer, isNeedBigIcon) {
				isNeedBigIcon = isNeedBigIcon || false;
				return getViewModel(imageContainer, isNeedBigIcon).container;
			}
		};
	};

	return fileUploaderComponent;
});