using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using TigerWord.Core.Biz;

namespace TigerWord.Core.Services
{
    public class SystemUtilityService
    {
        /// <summary>
        /// Create Module Information for hot load module
        /// 
        /// Rule 1) namespace might be FileName
        /// Rule 2) There might be FileName.Loader Class
        /// Rule 3) The class must implement TigerLoader interface
        /// 
        /// Then you can create GetModuleInfo() function to retrieve others to collect module info like name, icon, menu
        /// 
        /// </summary>
        /// <param name="filename"> Name of moudle</param>
        /// <returns>
        /// Seven values but last two is reserved for future use
        /// 
        /// 1st Type of moudle
        /// 2nd Name of moudle
        /// 3rd Tooltip of module
        /// 4th Icon of moudle
        /// 5th Menu of module (It replace existing menu except main menu)
        /// 6th Future use
        /// 7th Future use
        /// 
        /// </returns>
        public static Tuple<Type, string, string, PackIconBase, ObservableCollection<CoreMenuData>,object,object>? CreateModuleInfo(string filename)
        {
            string clr_name = Path.GetFileNameWithoutExtension(filename);
            string full_clr_name = filename;
            Assembly? clr_assembly = null;
            try
            {
                clr_assembly = Assembly.GetExecutingAssembly();
            }
            catch // (Exception ex)
            {
                return null;
            }
            try
            {
                clr_assembly = Assembly.LoadFrom(full_clr_name);
            }
            catch //(Exception ex2)
            {
                return null;
            }
            object? loaderOb = null;
            try
            {
                loaderOb = clr_assembly.CreateInstance(clr_name + ".Loader");
            }
            catch// (Exception ex3)
            {
                return null;
            }
            if (loaderOb == null)
                return null;
            TigerLoader? loader;
            try
            {
                loader = loaderOb as TigerLoader;
            }
            catch //(Exception ex4)
            {
                return null;
            }
            if (loader != null)
            {
                Tuple<Type, 
                    string, string, 
                    MahApps.Metro.IconPacks.PackIconBase,
                    ObservableCollection<CoreMenuData>,
                    object, object> label = loader.GetModuleInfo();
        Type target = label.Item1;
                Tuple<Type, string, string, PackIconBase, ObservableCollection<CoreMenuData>, object, object> moduleInfo = new 
                    Tuple<Type, string, string, PackIconBase, ObservableCollection<CoreMenuData>, object, object>
            (
            target,
            label.Item2,
            label.Item3,
            label.Item4,
            label.Item5,
            label.Item6,
            label.Item7
            
            );
                return moduleInfo;
            }
            return null;
        }

        /// <summary>
        /// 
        /// From the system Menu, remove all menu which is not system menu
        /// add application menu
        /// following order of sys_menu_order
        /// 
        /// dash(-) of menu_order indicate place of app menu
        /// 
        /// </summary>
        /// <param name="sys_menu_order"></param>
        /// <param name="system_module_id"></param>
        /// <param name="main_menu"></param>
        /// <param name="app_menu"></param>
        /// <returns></returns>
        public static  ObservableCollection<CoreMenuData>? ReorderMenu(Dictionary<string, string[]> sys_menu_order,string system_module_id,ObservableCollection<CoreMenuData> main_menu, ObservableCollection<CoreMenuData> app_menu)
        {

            //foreach(KeyValuePair<string, string[]> key_order in  menu_order)
            //{
            //    string key = key_order.Key;
            //    string[] order = key_order.Value as string[];
            //    if(key == "")
            //    {

            //    }
            //}
            ObservableCollection<CoreMenuData> new_main_menu = new ObservableCollection<CoreMenuData>();
            string[]? main_order = null;
            if (!sys_menu_order.ContainsKey("-"))
                return null;
            main_order = sys_menu_order["-"];
            bool app_done = false;
            //static string[] root_menu_order = { "file", "edit", "-", "tool", "window", "help" };
            if (main_order != null)
            {
                for (int i = 0; i < main_order.Length; i++)
                {
                    string menu_string = main_order[i];
                    if (menu_string != null)
                    {
                        if (main_order[i] == "-")//  "-"
                        {
                            if (!app_done)
                            {
                                int ll = 0;
                                IEnumerable<CoreMenuData>? finded_app_menudata = null;
                                try
                                {
                                    finded_app_menudata = app_menu.Where(x =>
                                    {
                                        for (int k = 0; k < main_order.Length; k++)
                                        {
                                            if (main_order[k] == x.ModuleID)
                                                return false;
                                        }
                                        return false;
                                    });
                                    ll = finded_app_menudata.Count();
                                }
                                catch { }
                                if (ll > 0)
                                {
                                    if (finded_app_menudata != null)
                                    {
                                        foreach (var menu_data in finded_app_menudata)
                                        {
                                            new_main_menu.Add(menu_data);
                                        }
                                    }
                                    app_done = true;
                                }
                            }
                        }
                        else//"file", "edit",, "tool", "window", "help" 
                        {
                            string[] menu_order = sys_menu_order[menu_string];

                            CoreMenuData? finded_main_menudata = null;
                            CoreMenuData? finded_app_menudata = null;
                            try
                            {
                                finded_main_menudata = main_menu.First(x => x.CommandID == menu_string);
                            }
                            catch { }
                            try
                            {
                                finded_app_menudata = app_menu.First(x => x.CommandID == menu_string);
                            }
                            catch { }
                            if (finded_app_menudata != null && finded_main_menudata != null)
                            {
                                ObservableCollection<CoreMenuData> exisingMenuList = finded_main_menudata.MenuItems;
                                ObservableCollection<CoreMenuData> replaceMenuList = finded_app_menudata.MenuItems;
                                ObservableCollection<CoreMenuData> new_menu_list = OverwriteMenu(system_module_id, exisingMenuList, replaceMenuList, menu_order);
                                finded_main_menudata.MenuItems = new_menu_list;
                                new_main_menu.Add(finded_main_menudata);
                            }
                            else if (finded_main_menudata != null)
                            {
                                new_main_menu.Add(finded_main_menudata);
                            }
                            else if (finded_app_menudata != null)
                            {
                                new_main_menu.Add(finded_app_menudata);
                            }
                            else
                            {
                                // never reach
                            }
                        }
                    }
                }
            }
            return new_main_menu;
        }

        /// <summary>
        /// From the existing Menu, remove which is not match existingMenuId
        /// add menus of replaceMenuList
        /// following order of menu_order
        /// 
        /// dash(-) of menu_order indicate place of added menu
        /// 
        /// </summary>
        /// <param name="existingMenuId"></param>
        /// <param name="exisingMenuList"></param>
        /// <param name="replaceMenuList"></param>
        /// <param name="menu_order"></param>
        /// <returns></returns>

        public static ObservableCollection<CoreMenuData> OverwriteMenu(
            string existingMenuId
            , ObservableCollection<CoreMenuData> exisingMenuList
            , ObservableCollection<CoreMenuData> replaceMenuList
            , string[] menu_order)
        {
            ObservableCollection<CoreMenuData> edited_menu = new ObservableCollection<CoreMenuData>();
            // erase existing other app Menu
            IEnumerable<CoreMenuData>? to_delete_menu_data = null;
            int n = 0;
            try
            {
                to_delete_menu_data = exisingMenuList.Where(x =>
                {

                    if (x.ModuleID == existingMenuId)
                        return false;
                    return true;
                });
                n = to_delete_menu_data.Count();
            }
            catch { }
            if (n > 0)
            {
                if (to_delete_menu_data != null)
                {
                    foreach (CoreMenuData dt in to_delete_menu_data)
                    {
                        exisingMenuList.Remove(dt);
                    }
                }
            }
            bool app_done = false;
            // insert replaceMenuList
            for (int j = 0; j < menu_order.Length; j++)
            {
                string select_menu = menu_order[j];

                //CoreMenuData finded_app_menudata = replaceMenuList.First(x => x.ModuleID == select_menu);
                if (select_menu == "-")
                {
                    int ln = 0;
                    if (!app_done)
                    {
                        IEnumerable<CoreMenuData>? finded_app_menudata = null;
                        try
                        {
                            finded_app_menudata = replaceMenuList.Where(x =>
                            {
                                for (int k = 0; k < menu_order.Length; k++)
                                {
                                    if (menu_order[k] == x.CommandID)
                                        return false;
                                }
                                return true;
                            });
                            ln = finded_app_menudata.Count();
                        }
                        catch { }
                        if (ln > 0)
                        {
                            if(finded_app_menudata !=null)
                            {
                                foreach (var menu_data in finded_app_menudata)
                                {
                                    edited_menu.Add(menu_data);
                                }
                            }
                        }
                    }
                    app_done = true;
                }
                else
                {
                    CoreMenuData? finded_main_menudata = null;
                    int menu_cnt = 0;
                    try
                    {
                        finded_main_menudata = exisingMenuList.First(x => x.CommandID == select_menu);
                        if (finded_main_menudata != null)
                            menu_cnt = 1;
                    }
                    catch { }

                    if (finded_main_menudata !=null && menu_cnt > 0)
                        edited_menu.Add(finded_main_menudata);
                }
            }
            return edited_menu;
        }
    }
}
