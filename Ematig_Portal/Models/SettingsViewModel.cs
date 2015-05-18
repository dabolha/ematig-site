using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ematig_Portal.Models
{
    public class SettingsViewModel
    {
        public IEnumerable<SettingsModel> SettingList { get; set; }

        public SettingsModel Setting { get; set; }
    }

    public class SettingsModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Chave")]
        public string Key { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Valor")]
        public string Value { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Descrição")]
        public string Name { get; set; }
    }
}