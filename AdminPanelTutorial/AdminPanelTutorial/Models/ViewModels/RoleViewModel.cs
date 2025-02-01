using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdminPanelTutorial.Models;

namespace AdminPanelTutorial.ViewModels
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        public int[] SelectedPermissions { get; set; } // For selected permissions

        public List<Permission> Permissions { get; set; } // All available permissions
    }
}
