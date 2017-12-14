using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace MyNameSpaceOfShellIcons
{
    public static class Icons
    {
        #region Custom exceptions class

        public class IconNotFoundException : Exception
        {
            public IconNotFoundException(string _fileName_, int _index_)
                : base(string.Format("Icon with Id = {0} wasn't found in file {1}", _index_, _fileName_))
            {
            }
        }

        public class UnableToExtractIconsException : Exception
        {
            public UnableToExtractIconsException(string _fileName_, int _firstIconIndex_, int _iconCount_)
                : base(string.Format("Tryed to extract {2} icons starting from the one with id {1} from the \"{0}\" file but failed", _fileName_, _firstIconIndex_, _iconCount_))
            {
            }
        }

        #endregion

        #region DllImports

        /// <summary>
        /// Contains information about a file object. 
        /// </summary>
        struct Shfileinfo
        {
            /// <summary>
            /// Handle to the icon that represents the file. You are responsible for
            /// destroying this handle with DestroyIcon when you no longer need it. 
            /// </summary>
#pragma warning disable 649
            public IntPtr hIcon;
#pragma warning restore 649

            /// <summary>
            /// Index of the icon image within the system image list.
            /// </summary>
#pragma warning disable 169
            public IntPtr iIcon;
#pragma warning restore 169

            /// <summary>
            /// Array of values that indicates the attributes of the file object.
            /// For information about these values, see the IShellFolder::GetAttributesOf
            /// method.
            /// </summary>
#pragma warning disable 169
            public uint dwAttributes;
#pragma warning restore 169

            /// <summary>
            /// String that contains the name of the file as it appears in the Microsoft
            /// Windows Shell, or the path and file name of the file that contains the
            /// icon representing the file.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
#pragma warning disable 169
            public string szDisplayName;
#pragma warning restore 169

            /// <summary>
            /// String that describes the type of file.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
#pragma warning disable 169
            public string szTypeName;
#pragma warning restore 169
        };

        [Flags]
        enum FileInfoFlags
        {
            /// <summary>
            /// Retrieve the handle to the icon that represents the file and the index 
            /// of the icon within the system image list. The handle is copied to the 
            /// hIcon member of the structure specified by psfi, and the index is copied 
            /// to the iIcon member.
            /// </summary>
            ShgfiIcon = 0x000000100,
            /// <summary>
            /// Indicates that the function should not attempt to access the file 
            /// specified by pszPath. Rather, it should act as if the file specified by 
            /// pszPath exists with the file attributes passed in dwFileAttributes.
            /// </summary>
            ShgfiUsefileattributes = 0x000000010
        }

        /// <summary>
        ///     Creates an array of handles to large or small icons extracted from
        ///     the specified executable file, dynamic-link library (DLL), or icon
        ///     file. 
        /// </summary>
        /// <param name="_lpszFile_">
        ///     Name of an executable file, DLL, or icon file from which icons will
        ///     be extracted.
        /// </param>
        /// <param name="_nIconIndex_">
        ///     <para>
        ///         Specifies the zero-based index of the first icon to extract. For
        ///         example, if this value is zero, the function extracts the first
        ///         icon in the specified file.
        ///     </para>
        ///     <para>
        ///         If this value is �1 and <paramref>
        ///             <name>phiconLarge</name>
        ///         </paramref>
        ///         and
        ///         <paramref>
        ///             <name>phiconSmall</name>
        ///         </paramref>
        ///         are both NULL, the function returns
        ///         the total number of icons in the specified file. If the file is an
        ///         executable file or DLL, the return value is the number of
        ///         RT_GROUP_ICON resources. If the file is an .ico file, the return
        ///         value is 1. 
        ///     </para>
        ///     <para>
        ///         Windows 95/98/Me, Windows NT 4.0 and later: If this value is a 
        ///         negative number and either <paramref>
        ///             <name>phiconLarge</name>
        ///         </paramref>
        ///         or 
        ///         <paramref name="phiconSmall"/> is not NULL, the function begins by
        ///         extracting the icon whose resource identifier is equal to the
        ///         absolute value of <paramref name="_nIconIndex_"/>. For example, use -3
        ///         to extract the icon whose resource identifier is 3. 
        ///     </para>
        /// </param>
        /// <param name="_phIconLarge_">
        ///     An array of icon handles that receives handles to the large icons
        ///     extracted from the file. If this parameter is NULL, no large icons
        ///     are extracted from the file.
        /// </param>
        /// <param name="_phIconSmall_">
        ///     An array of icon handles that receives handles to the small icons
        ///     extracted from the file. If this parameter is NULL, no small icons
        ///     are extracted from the file. 
        /// </param>
        /// <param name="_nIcons_">
        ///     Specifies the number of icons to extract from the file. 
        /// </param>
        /// <returns>
        ///     If the <paramref name="_nIconIndex_"/> parameter is -1, the
        ///     <paramref name="_phIconLarge_"/> parameter is NULL, and the
        ///     <paramref name="phiconSmall"/> parameter is NULL, then the return
        ///     value is the number of icons contained in the specified file.
        ///     Otherwise, the return value is the number of icons successfully
        ///     extracted from the file. 
        /// </returns>
        [DllImport("Shell32", CharSet = CharSet.Auto)]
        extern static int ExtractIconEx(
            [MarshalAs(UnmanagedType.LPTStr)] 
            string _lpszFile_,
            int _nIconIndex_,
            IntPtr[] _phIconLarge_,
            IntPtr[] _phIconSmall_,
            int _nIcons_);

        [DllImport("Shell32", CharSet = CharSet.Auto)]
        extern static IntPtr SHGetFileInfo(
            string _pszPath_,
            int _dwFileAttributes_,
            out Shfileinfo _psfi_,
            int _cbFileInfo_,
            FileInfoFlags _uFlags_);

        #endregion

        /// <summary>
        /// Two constants extracted from the FileInfoFlags, the only that are
        /// meaningfull for the user of this class.
        /// </summary>
        public enum SystemIconSize
        {
            Large = 0x000000000,
            Small = 0x000000001
        }

        /// <summary>
        /// Get the number of icons in the specified file.
        /// </summary>
        /// <param name="_fileName_">Full path of the file to look for.</param>
        /// <returns></returns>
        static int GetIconsCountInFile(string _fileName_)
        {
            return ExtractIconEx(_fileName_, -1, null, null, 0);
        }

        #region ExtractIcon-like functions

        public static void ExtractEx(string _fileName_, List<Icon> _largeIcons_,
            List<Icon> _smallIcons_, int _firstIconIndex_, int _iconCount_)
        {
            /*
             * Memory allocations
             */

            IntPtr[] smallIconsPtrs = null;
            IntPtr[] largeIconsPtrs = null;

            if (_smallIcons_ != null)
            {
                smallIconsPtrs = new IntPtr[_iconCount_];
            }
            if (_largeIcons_ != null)
            {
                largeIconsPtrs = new IntPtr[_iconCount_];
            }

            /*
             * Call to native Win32 API
             */

            int apiResult = ExtractIconEx(_fileName_, _firstIconIndex_, largeIconsPtrs, smallIconsPtrs, _iconCount_);
            if (apiResult != _iconCount_)
            {
                throw new UnableToExtractIconsException(_fileName_, _firstIconIndex_, _iconCount_);
            }

            /*
             * Fill lists
             */

            if (_smallIcons_ != null)
            {
                _smallIcons_.Clear();
                foreach (IntPtr actualIconPtr in smallIconsPtrs)
                {
                    _smallIcons_.Add(Icon.FromHandle(actualIconPtr));
                }
            }
            if (_largeIcons_ != null)
            {
                _largeIcons_.Clear();
                foreach (IntPtr actualIconPtr in largeIconsPtrs)
                {
                    _largeIcons_.Add(Icon.FromHandle(actualIconPtr));
                }
            }
        }

        public static List<Icon> ExtractEx(string _fileName_, SystemIconSize _size_,
            int _firstIconIndex_, int _iconCount_)
        {
            List<Icon> iconList = new List<Icon>();

            switch (_size_)
            {
                case SystemIconSize.Large:
                    ExtractEx(_fileName_, iconList, null, _firstIconIndex_, _iconCount_);
                    break;

                case SystemIconSize.Small:
                    ExtractEx(_fileName_, null, iconList, _firstIconIndex_, _iconCount_);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("_size_");
            }

            return iconList;
        }

        public static void Extract(string _fileName_, List<Icon> _largeIcons_, List<Icon> _smallIcons_)
        {
            int iconCount = GetIconsCountInFile(_fileName_);
            ExtractEx(_fileName_, _largeIcons_, _smallIcons_, 0, iconCount);
        }

        public static List<Icon> Extract(string _fileName_, SystemIconSize _size_)
        {
            int iconCount = GetIconsCountInFile(_fileName_);
            return ExtractEx(_fileName_, _size_, 0, iconCount);
        }

        public static Icon ExtractOne(string _fileName_, int _index_, SystemIconSize _size_)
        {
            try
            {
                List<Icon> iconList = ExtractEx(_fileName_, _size_, _index_, 1);
                return iconList[0];
            }
            catch (UnableToExtractIconsException)
            {
                throw new IconNotFoundException(_fileName_, _index_);
            }
        }

        public static void ExtractOne(string _fileName_, int _index_,
            out Icon _largeIcon_, out Icon _smallIcon_)
        {
            List<Icon> smallIconList = new List<Icon>();
            List<Icon> largeIconList = new List<Icon>();
            try
            {
                ExtractEx(_fileName_, largeIconList, smallIconList, _index_, 1);
                _largeIcon_ = largeIconList[0];
                _smallIcon_ = smallIconList[0];
            }
            catch (UnableToExtractIconsException)
            {
                throw new IconNotFoundException(_fileName_, _index_);
            }
        }

        #endregion

        //this will look throw the registry 
        //to find if the Extension have an icon.
        public static Icon IconFromExtension(string _extension_,
                                                SystemIconSize _size_)
        {
            // Add the '.' to the extension if needed
            if (_extension_[0] != '.') _extension_ = '.' + _extension_;

            //opens the registry for the wanted key.
            RegistryKey root = Registry.ClassesRoot;
            RegistryKey extensionKey = root.OpenSubKey(_extension_);
            if (extensionKey != null)
            {
                extensionKey.GetValueNames();
                RegistryKey applicationKey =
                    root.OpenSubKey(extensionKey.GetValue("").ToString());

                //gets the name of the file that have the icon.
                if (applicationKey != null)
                {
                    string iconLocation =
                        applicationKey.OpenSubKey("DefaultIcon").GetValue("").ToString();
                    List<string> iconPath = iconLocation.Split(',').ToList();
                    if (iconPath.Count==1)
                    {
                        iconPath.Add("0");
                    }
                    if (iconPath[1] == null) iconPath[1] = "0";
                    IntPtr[] large = new IntPtr[1], small = new IntPtr[1];

                    //extracts the icon from the file.
                    ExtractIconEx(iconPath[0],
                        Convert.ToInt16(iconPath[1]), large, small, 1);
                    return _size_ == SystemIconSize.Large ?
                        Icon.FromHandle(large[0]) : Icon.FromHandle(small[0]);
                }
            }
            return null;
        }

        public static Icon IconFromExtensionShell(string _extension_, SystemIconSize _size_)
        {
            //add '.' if nessesry
            if (_extension_[0] != '.') _extension_ = '.' + _extension_;

            //temp struct for getting file shell info
            Shfileinfo fileInfo = new Shfileinfo();

            SHGetFileInfo(
                _extension_,
                0,
                out fileInfo,
                Marshal.SizeOf(fileInfo),
                FileInfoFlags.ShgfiIcon | FileInfoFlags.ShgfiUsefileattributes | (FileInfoFlags)_size_);

            return Icon.FromHandle(fileInfo.hIcon);
        }

        public static Icon IconFromResource(string _resourceName_)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            return new Icon(assembly.GetManifestResourceStream(_resourceName_));
        }

        /// <summary>
        /// Parse strings in registry who contains the name of the icon and
        /// the index of the icon an return both parts.
        /// </summary>
        /// <param name="_regString_">The full string in the form "path,index" as found in registry.</param>
        /// <param name="_fileName_">The "path" part of the string.</param>
        /// <param name="_index_">The "index" part of the string.</param>
        public static void ExtractInformationsFromRegistryString(
            string _regString_, out string _fileName_, out int _index_)
        {
            if (_regString_ == null)
            {
                throw new ArgumentNullException("_regString_");
            }
            if (_regString_.Length == 0)
            {
                throw new ArgumentException("The string should not be empty.", "_regString_");
            }

            _index_ = 0;
            string[] strArr = _regString_.Replace("\"", "").Split(',');
            _fileName_ = strArr[0].Trim();
            if (strArr.Length > 1)
            {
                int.TryParse(strArr[1].Trim(), out _index_);
            }
        }

        public static Icon ExtractFromRegistryString(string _regString_, SystemIconSize _size_)
        {
            string fileName;
            int index;
            ExtractInformationsFromRegistryString(_regString_, out fileName, out index);
            return ExtractOne(fileName, index, _size_);
        }
    }
}

