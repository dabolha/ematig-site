using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ematig_Portal.Models
{
    public class SendMessageViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Nome")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Sobrenome")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Endereço eletrónico")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Telefone")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Assunto")]
        public string Title { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Mensagem")]
        public string Message { get; set; }

    }
}
