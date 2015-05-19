using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ematig_Portal.Models
{
    public class MessageViewModel
    {
        public IEnumerable<ViewMessageModel> MessageList { get; set; }

        public ViewMessageModel Message { get; set; }
    }

    public class ViewMessageModel
    {
        public long Id { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Endereço eletrónico")]
        public string ToEmail { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Assunto")]
        public string Subject { get; set; }

        public short MessageTypeID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Mensagem")]
        public string Message { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data de criação")]
        public DateTime CreationDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data de envio")]
        public DateTime? SentDate { get; set; }
    }

    public class MessageModel
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

        [DataType(DataType.DateTime)]
        [Display(Name = "Data de criação")]
        public DateTime CreationDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data de envio")]
        public DateTime? SentDate { get; set; }
    }
}
