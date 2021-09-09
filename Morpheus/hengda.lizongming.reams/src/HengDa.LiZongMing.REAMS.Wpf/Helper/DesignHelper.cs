using System;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows;

namespace Aming.Core
{
    public static class DesignHelper
    {
        #region Fields

        private static bool? _isInDesignMode;
        public static readonly Random Rand = new Random(Guid.NewGuid().GetHashCode());

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running in Blend or Visual Studio).
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
            Justification = "The security risk here is neglectible.")]
        public static bool IsInDesignMode
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
#if SILVERLIGHT
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
#else
#if WIN8
                    _isInDesignMode = Windows.ApplicationModel.DesignMode.DesignModeEnabled;
#else
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    _isInDesignMode
                        = (bool)DependencyPropertyDescriptor
                                     .FromProperty(prop, typeof(FrameworkElement))
                                     .Metadata.DefaultValue;

                    // Just to be sure
                    if (!_isInDesignMode.Value
                        && Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal))
                    {
                        _isInDesignMode = true;
                    }
#endif
#endif
                }

                return _isInDesignMode.Value;
            }
        }

        #endregion // Properties

        #region Public Methods
        /// <summary>
        /// 生成随机字母 given its minimum length and maximum length
        /// </summary>
        public static string GetRandomString(int minLen = 15, int maxLen = 50)
        {
            StringBuilder builder = new StringBuilder();
            int length = Rand.Next(minLen, maxLen);
            char ch;
            for (int i = 0; i < length; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Rand.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString().ToLower();
        }
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="minLen"></param>
        /// <param name="maxLen"></param>
        /// <returns></returns>
        public static string GetRandomNumericString(int minLen = 15, int maxLen = 25)
        {
            StringBuilder builder = new StringBuilder();
            int length = Rand.Next(minLen, maxLen);
            for (int i = 0; i < length; i++)
            {
                builder.Append(Rand.Next(0, 10).ToString());
            }

            return builder.ToString().ToLower();
        }

        #endregion // Public Methods
    }
}
