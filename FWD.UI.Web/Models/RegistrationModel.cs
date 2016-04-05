using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FWD.UI.Web.Models
{
	public class RegistrationModel
	{
		[Required(ErrorMessage = "Поле \"Почта\" не должно быть пустым")]
		[MinLength(5, ErrorMessage = "Длинна поля \"Почта\" должны превышать 4 символа")]
		[RegularExpression(@"^[-\w.]+@([A-z0-9][-A-z0-9]+\.)+[A-z]{2,4}$", ErrorMessage = "Что-то не похоже на адрес почты")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Поле \"Имя\" не должно быть пустым")]
		[MinLength(3, ErrorMessage = "Длинна поля \"Имя\" должны превышать 2 символа")]
		[RegularExpression("[а-яА-Я]*", ErrorMessage = "Поле \"Имя\" должно состоять только из русских букв")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Поле \"Фамилия\" не должно быть пустым")]
		[MinLength(2, ErrorMessage = "Длинна поля \"Фамилия\" должны превышать 1 символ")]
		[RegularExpression("[а-яА-Я]*", ErrorMessage = "Поле \"Фамилия\" должно состоять только из русских букв")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Поле \"Логин\" не должно быть пустым")]
		[MinLength(6, ErrorMessage = "Длинна поля \"Логин\" должны превышать 5 символ")]
		[RegularExpression(@"^[a-zA-Z][a-zA-Z0-9-_\.]{6,20}$", ErrorMessage = "Поле \"Логин\" должно состоять только латинских букв,цифр. Ограничение поля 6-20 символов")]//
		public string Login { get; set; }

		[Required(ErrorMessage = "Поле \"Пароль\" не должно быть пустым")]
		[MinLength(6, ErrorMessage = "Длинна поля \"Пароль\" должны превышать 5 символ")]
		[RegularExpression(@"(?=^.{6,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Поле \"Пароль\" должно состоять только латинских букв,цифр и спецсимволов")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Поле \"Повторите пароль\" не должно быть пустым")]
		[MinLength(6, ErrorMessage = "Длинна поля \"Повторите пароль\" должны превышать 5 символ")]
		public string ConfirmPassword { get; set; }
	}
}