using MarkdownAnimator.Properties;
using Miktemk;
using Miktemk.Winforms.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownAnimator.Services
{
    public interface IRegistryService
    {
        SublimeStyleFoldersVM SidebarFolders { get; }
    }
    public class RegistryService : IRegistryService
    {
        public const string RegFolders = "SidebarFolders";

        public RegistryService()
        {

        }

        public SublimeStyleFoldersVM _sidebarFolders;
        public SublimeStyleFoldersVM SidebarFolders
        {
            get
            {
                if (_sidebarFolders != null)
                    return _sidebarFolders;
                _sidebarFolders = UtilsRegistry
                    .OpenUserSoftwareKey(Settings.Default.RegRoot)
                    .GetValueDoubleXml<SublimeStyleFoldersVM>(RegFolders);
                _sidebarFolders.Changed += () =>
                {
                    UtilsRegistry
                        .OpenUserSoftwareKey(Settings.Default.RegRoot)
                        .SetValueDoubleXml<SublimeStyleFoldersVM>(RegFolders, _sidebarFolders);
                };
                return _sidebarFolders;
            }
        }
    }

}
