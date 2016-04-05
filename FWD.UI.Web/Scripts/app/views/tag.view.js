define([
	'jquery',
	'app/helpers/notification.static.helper',
	'app/helpers/configurator.static.helper',
	'app/models/tag.model'
], function ($, errorHelper,conf, TagModel) {
	var tagView = function () {

		var model = { model: new TagModel(), content: {}, tags: [], updatingItem: {} };


		var init = function () {
			model.content = $('<div class="tag_content"></div>');
			model.tagInfo = {};
			model.rowTemplate = '<div class="tag_row_editor"></div>';

			var tagColors = $('<select class="tag_colors"/>');
			var tagTypes = $('<input class="tag_types" type="text" disabled/>');
			var rowEditor = $(model.rowTemplate);

			model.tagTypeTamplate = '<div class="tag_type"></div>';
			model.tagNameTemplate = '<div class="tag_name"></div>';
			model.tagColorTemplate = '<div class="tag_color"></div>';
			model.tagPriorityTemplate = '<div class="tag_priority"></div>';

			model.tagPreview = $('<div class="tag_preview"></div>');
			model.tagPreview.append($('<a class="tag"></a>'));

			model.tagText = $('<input type="text" class="tag_text"/>');
			model.tagPriority = $('<select class="tag_priority"/>');
			model.tagAddNew = $('<button class="tag_add_new">+</button>');

			rowEditor.append(tagColors);
			rowEditor.append(tagTypes);
			rowEditor.append(model.tagPriority);
			rowEditor.append(model.tagText);
			rowEditor.append(model.tagPreview);
			rowEditor.append(model.tagAddNew);

			model.tagPriority.append($('<option value="0">0</option>'));
			model.tagPriority.append($('<option value="1">1</option>'));
			model.tagPriority.append($('<option value="2">2</option>'));
			model.tagPriority.append($('<option value="3">3</option>'));
			model.tagPriority.append($('<option value="4">4</option>'));
			model.tagPriority.append($('<option value="5">5</option>'));

			model.tagColors = tagColors;
			model.tagTypes = tagTypes;

			model.tagText.off('keyup');

			model.tagText.on('keyup', function () {
				var text = $(this).val();
				model.tagPreview.children().text(text);
			});

			model.tagAddNew.off('click');
			model.tagAddNew.on('click', function () {
				var entity = getModelFromControls();
				var isValid = validate(entity);
				if (isValid) {
					model.model.save(model.content, entity)
						.success(function (id) {
							errorHelper.showInfo('Тэг "' + entity.name + '" успешно сохранен');
							var item = {
								Id: id,
								Name: entity.name,
								Priority: entity.priority,
								TagColor: entity.tagColor,
								TagType: entity.tagType
							};
							model.tagInfo.Tags.push(item);
							model.tags.push(item);
							appendTagToView(item, model.currentTags);
							appendOptionToExistingTagList(id, entity.name);
						})
						.fail(function (ctx) {
							errorHelper.showError(ctx.responseText);
						});
				}
			});

			var rowUpdater = $(model.rowTemplate);

			model.addCurrentTag = $('<button class="add_current_tag">Добавить</button>');
			model.deleteTag = $('<button class="delete_tag">Удалить</button>');
			model.updateTag = $('<button class="update_tag">Обновить</button>');
			model.existing = $('<select class="tag_existing"/>');
			rowUpdater.append(model.existing);
			rowUpdater.append(model.addCurrentTag);
			rowUpdater.append(model.deleteTag);
			rowUpdater.append(model.updateTag);

			var rowUpdateApplying = $(model.rowTemplate);
			rowUpdateApplying.addClass('invisible');
			model.tagPreview_update = $('<div class="tag_preview update"></div>');
			model.tagPreview_update.append($('<a class="tag"></a>'));

			model.tagText_update = $('<input type="text" class="tag_text update"/>');
			model.tagPriority_update = $('<select class="tag_priority update"/>');
			model.tagUpdateApply = $('<button class="tag_update_apply">&#709;</button>');
			model.tagColors_update = $('<select class="tag_colors update"/>');
			model.tagTypes_update = $('<input class="tag_types update" type="text" disabled/>');

			model.tagPriority_update.append($('<option value="0">0</option>'));
			model.tagPriority_update.append($('<option value="1">1</option>'));
			model.tagPriority_update.append($('<option value="2">2</option>'));
			model.tagPriority_update.append($('<option value="3">3</option>'));
			model.tagPriority_update.append($('<option value="4">4</option>'));
			model.tagPriority_update.append($('<option value="5">5</option>'));

			rowUpdateApplying.append(model.tagColors_update);
			rowUpdateApplying.append(model.tagTypes_update);
			rowUpdateApplying.append(model.tagPriority_update);
			rowUpdateApplying.append(model.tagText_update);
			rowUpdateApplying.append(model.tagPreview_update);
			rowUpdateApplying.append(model.tagUpdateApply);


			model.tagText_update.off('keyup');

			model.tagText_update.on('keyup', function () {
				var text = $(this).val();
				model.tagPreview_update.children().text(text);
			});

			var setValues = function (item) {

				model.tagText_update.val(item.Name);
				model.tagPriority_update.val(item.Priority);
				model.tagTypes_update.val(item.TagType == '0' ? "Standard" : "Undefind");

				model.tagColors_update.val(item.TagColor);

				model.tagPreview_update.children().text(item.Name);
				_.each(model.tagInfo.TagColors, function (itm) {
					model.tagPreview_update.children().removeClass(itm);
				});
				model.tagPreview_update.children().addClass(item.TagColor);
				model.updatingItem = item;

				if (model.updateApplier.hasClass('invisible')) {
					model.updateApplier.removeClass('invisible');
					model.updateApplier.addClass('animated fadeInDown');
					model.updateApplier.off('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend');
					model.updateApplier.on('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
						model.updateApplier.removeClass('invisible');
						model.updateApplier.removeClass('animated');
						model.updateApplier.removeClass('fadeInDown');
					});
				} else {
					model.updateApplier.addClass('animated fadeOutUp');

					model.updateApplier.off('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend');
					model.updateApplier.on('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
						model.updateApplier.addClass('invisible');
						model.updateApplier.removeClass('animated');
						model.updateApplier.removeClass('fadeOutUp');
					});
				}
			};

			model.updateTag.off('click');
			model.updateTag.on('click', function () {
				var id = model.existing.val();
				var item = _.find(model.tags, function (current) {
					return current.Id == id;
				});

				if (item == undefined) {
					model.model.getTag(model.content, id)
						.success(function (tag) {
							item = tag;
							setValues(item);
						}).
						fail(function (ex) {
							errorHelper.showError(ex.responseText);
						});
				} else {
					setValues(item);
				}
			});

			model.tagUpdateApply.off('click');
			model.tagUpdateApply.on('click', function () {
				if (model.updatingItem != undefined) {

					var current = getModelFromControls(true);
					var isValid = validate(current, true);
					if (isValid) {
						current.id = model.updatingItem.Id;
						model.model.update(model.content, current)
							.success(function () {
								model.updateApplier.addClass('animated fadeOutUp');

								model.updateApplier.off('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend');
								model.updateApplier.on('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
									model.updateApplier.addClass('invisible');
									model.updateApplier.removeClass('animated');
									model.updateApplier.removeClass('fadeOutUp');
									errorHelper.showInfo('Тэг "' + current.name + '" успешно обновлен');

									var opt = $(model.existing.find('option[value=' + current.id + ']'));
									opt.text(current.name);

									var tag = { Id: current.id, Name: current.name, Priority: current.priority, TagColor: current.tagColor, TagType: current.tagType };
									updateTagOnView(tag, model.currentTags);

									var tagIndex = _.toArray(model.tags).indexOf(model.updatingItem);
									if (tagIndex != -1)
										model.tags[tagIndex] = tag;

									tagIndex = _.toArray(model.tagInfo.Tags).indexOf(model.updatingItem);
									if (tagIndex != -1)
										model.tags[tagIndex] = tag;
								});

							}).
							fail(function (ex) {
								errorHelper.showError(ex.responseText);
							});
					}
				}
			});

			model.addCurrentTag.off('click');
			model.addCurrentTag.on('click', function () {
				var id = model.existing.val();
				var item = _.find(model.tags, function (current) {
					return current.Id == id;
				});
				if (item == undefined || item.length == 0) {
					model.model.getTag(model.content, id)
						.success(function (tag) {
							model.tags.push(tag);
							appendTagToView(tag, model.currentTags);
						}).
						fail(function (ex) {
							errorHelper.showError(ex.responseText);
						});
				} else {
					errorHelper.showError('Тэг "' + item.Name + '" уже присутствует у статьи');
				}
			});

			model.deleteTag.off('click');
			model.deleteTag.on('click', function () {
				var id = model.existing.val();
				var item = _.find(model.tags, function (current) {
					return current.Id == id;
				});

				model.model.delete(model.content, id)
					.success(function () {
						if (item != undefined) {
							errorHelper.showInfo('Тэг "' + item.Name + '" успешно удален');

							var tagIndex = _.toArray(model.tags).indexOf(item);
							model.tags.splice(tagIndex, 1);

							tagIndex = _.toArray(model.tagInfo.Tags).indexOf(item);
							model.tagInfo.Tags.splice(tagIndex);

							$(model.existing.find('option[value=' + id + ']')).remove();
							$(model.currentView.find('a[id=' + id + ']')).remove();
						} else {
							var element = $(model.existing.find('option[value=' + id + ']'));
							errorHelper.showInfo('Тэг "' + element.text() + '" успешно удален');
							element.remove();
						}

					}).
					fail(function (ex) {
						errorHelper.showError(ex.responseText);
					});
			});

			var rowCurrentTags = $(model.rowTemplate);
			model.currentTags = $('<div class="tag_preview"></div>');
			rowCurrentTags.append(model.currentTags);

			model.editor = rowEditor;
			model.updater = rowUpdater;
			model.currentView = rowCurrentTags;
			model.updateApplier = rowUpdateApplying;
		};

		var appendOptionToExistingTagList = function (id, name) {
			var select = $('<option/>');
			select.attr('value', id);
			select.text(name);
			model.existing.append(select);
		};

		var getModelFromControls = function (forUpdate) {

			forUpdate = forUpdate || false;

			var name = forUpdate ? $('.tag_text.update') : $('.tag_text');
			var tagType = forUpdate ? $('.tag_types.update') : $('.tag_types');
			var tagColor = forUpdate ? $('.tag_colors.update') : $('.tag_colors');
			var tagPriority = forUpdate ? $('.tag_priority.update') : $('.tag_priority');

			name.removeClass(conf.errorClass);
			tagType.removeClass(conf.errorClass);
			tagColor.removeClass(conf.errorClass);
			tagPriority.removeClass(conf.errorClass);

			var currentModel = {};
			currentModel.priority = tagPriority.val();
			currentModel.tagColor = tagColor.val();
			currentModel.tagType = tagType.val();
			currentModel.name = name.val();
			return currentModel;
		};

		var validate = function (entity, forEdit) {
			var errors = [];
			forEdit = forEdit || false;

			if (entity.priority < 0 || entity.priority > 5) {
				errors.push('Приоритет тэга не может быть меньше 0 или больше 5');
				if (!forEdit)
					$('.tag_priority').addClass(conf.errorClass);
				else
					$('.tag_priority.update').addClass(conf.errorClass);

			}

			if (entity.name == undefined || entity.name.length < 1) {
				errors.push('Название тэга должно содержать хотя бы один символ');
				if (!forEdit)
					$('.tag_text').addClass(conf.errorClass);
				else
					$('.tag_text.update').addClass(conf.errorClass);

			}

			if (errors.length > 0) {

				var message = '';
				_.each(errors, function (item) {
					message += item + '. ';
				});
				errorHelper.showError(message);
				return false;
			}
			return true;
		};

		var postInit = function (rootElement) {
			return model.model.getAll(rootElement)
				.success(function (info) {
					model.tagInfo = info;

					model.tagColors.off('change');
					model.tagColors.on('change', function () {

						_.each(model.tagInfo.TagColors, function (item) {
							model.tagPreview.children().removeClass(item);
						});
						model.tagPreview.children().addClass($(this).val());
					});

					model.tagColors_update.off('change');
					model.tagColors_update.on('change', function () {

						_.each(model.tagInfo.TagColors, function (item) {
							model.tagPreview_update.children().removeClass(item);
						});
						model.tagPreview_update.children().addClass($(this).val());
					});
				})
			.fail(function (ex) {
				errorHelper.showError(ex.responseText);
			});
		};

		var renderForAdd = function (view) {
			_.each(model.tagInfo.Tags, function (item) {
				appendOptionToExistingTagList(item.Id, item.Name);
			});
			model.tags = [];
			model.deleteTag.css('display', 'none');
			model.updateTag.css('display', 'none');

			model.addCurrentTag.off('click');
			model.addCurrentTag.on('click', function () {
				var id = model.existing.val();
				var item = _.find(model.tags, function (current) {
					return current.Id == id;
				});
				if (item == undefined || item.length == 0) {
					model.model.getTag(model.content, id)
						.success(function (tag) {
							model.tags.push(tag);
							appendTagToView(tag, view);
						}).
						fail(function (ex) {
							errorHelper.showError(ex.responseText);
						});
				} else {
					errorHelper.showError('Тэг "' + item.Name + '" уже присутствует');
				}
			});
			model.content.append(model.updater);
			return model.content;
		};

		var render = function (isFail) {
			_.each(model.tagInfo.TagColors, function (item) {
				var select = $('<option/>');
				select.attr('value', item);
				select.text(item);

				var select1 = $('<option/>');
				select1.attr('value', item);
				select1.text(item);

				model.tagColors.append(select);
				model.tagColors_update.append(select1);
			});

			_.each(model.tagInfo.TagTypes, function (item) {
				model.tagTypes.val(item);
				model.tagTypes_update.val(item);
			});

			_.each(model.tagInfo.Tags, function (item) {
				appendOptionToExistingTagList(item.Id, item.Name);
			});

			if (isFail) {
				model.tagPriority.attr('disabled', 'true');
				model.tagText.attr('disabled', 'true');
				model.tagColors.attr('disabled', 'true');
				model.tagPreview.attr('disabled', 'true');
				model.tagAddNew.attr('disabled', 'true');

				model.existing.attr('disabled', 'true');
			}

			$(model.tagPreview.children()[0]).addClass(model.tagColors.val());
			$(model.tagPreview_update.children()[0]).addClass(model.tagColors_update.val());

			model.content.append(model.editor);
			model.content.append(model.updater);
			model.content.append(model.updateApplier);
			model.content.append(model.currentView);

			return model.content;
		};

		var appendExisting = function (items) {
			model.tags = items;
			_.each(items, function (item) {
				appendTagToView(item, model.currentTags);
			});
		};

		var appendTagToView = function (item, view, needToSetBehaviour) {
			needToSetBehaviour = needToSetBehaviour || false;
			var a = $('<a class="tag"></a>');
			a.addClass(item.TagColor);
			a.text(item.Name);
			a.attr('id', item.Id);
			a.attr('tagColor', item.TagColor);
			a.attr('tagType', item.TagType);
			a.attr('priority', item.Priority);

			if (!needToSetBehaviour) {
				a.off('mouseenter');
				a.on('mouseenter', function () {
					var existing = $(view.find('.small_delete_tag_btn[id=' + $(this).attr('id') + ']'));
					if (existing.length == 0) {
						var smallDeleteBtn = $('<button id="' + item.Id + '" class="small_delete_tag_btn">x</button>');
						smallDeleteBtn.off('click');
						smallDeleteBtn.on('click', function () {

							var that = $(this);
							var id = that.attr('id');
							var currentTag = $(view.find('a[id=' + id + ']'));

							currentTag.remove();

							var tagToDelete = _.filter(model.tags, function (tag) {
								return tag.Id == id;
							});

							var deleteIndex = _.toArray(model.tags).indexOf(tagToDelete[0]);
							model.tags.splice(deleteIndex, 1);
							that.remove();
						});
						a.append(smallDeleteBtn);
					}
				});

				a.off('mouseleave');
				a.on('mouseleave', function () {
					var existing = $(view.find('.small_delete_tag_btn[id=' + $(this).attr('id') + ']'));
					setTimeout(function () {
						existing.remove();
					}, 1000);
				});
			}
			/*model.currentTags.append(a);*/
			view.append(a);
		};

		var updateTagOnView = function (item, view) {

			var a = $(view.find('a[id=' + item.Id + ']'));
			if (a.length > 0) {
				a.text(item.Name);
				a.attr('id', item.Id);
				a.attr('tagColor', item.TagColor);
				a.attr('tagType', item.TagType);
				a.attr('priority', item.Priority);

				_.each(model.tagInfo.TagColors, function (color) {
					a.removeClass(color);
				});
				a.addClass(item.TagColor);
			}
		};

		return {
			init: function (rootElement) {
				init();
				return postInit(rootElement);
			},

			render: function (isFail) {
				return render(isFail);
			},

			renderForAdd: function (view) {
				return renderForAdd(view);
			},

			appendExisting: function (items) {
				appendExisting(items);
			},

			getModel: function () {
				return model;
			},

			generateTags: function (tags, view) {
				_.each(tags, function (item) {
					appendTagToView(item, view, true);
				});
			}
		};
	};

	return tagView;
});