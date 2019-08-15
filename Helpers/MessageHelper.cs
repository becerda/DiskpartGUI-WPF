using DiskpartGUI.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiskpartGUI.Helpers
{
    static class MessageHelper
    {
        /// <summary>
        /// Shows a MessageBox with an error
        /// </summary>
        /// <param name="callingfrom">Where the error happend</param>
        /// /// <param name="exitcode">The exit code</param>
        /// <param name="errormessage">The error message</param>
        public static void ShowError(string callingfrom, ProcessExitCode exitcode, string errormessage)
        {
            ShowError(callingfrom, exitcode, errormessage, string.Empty);
        }

        /// <summary>
        /// Shows a MessageBox with an error
        /// </summary>
        /// <param name="callingfrom">Where the error happend</param>
        /// <param name="exitcode">The exit code</param>
        /// <param name="errormessage">The error message</param>
        /// <param name="extra">Extra message to add on</param>
        public static void ShowError(string callingfrom, ProcessExitCode exitcode, string errormessage, string extra)
        {
            MessageBox.Show("Error: " + exitcode + " - " + errormessage +
                "\n" + extra, callingfrom);
        }

        /// <summary>
        /// Shows success MessageBox
        /// </summary>
        /// <param name="message">The message to show</param>
        public static void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Success!");
        }

        /// <summary>
        /// Shows failure MessageBox
        /// </summary>
        /// <param name="message">The message to show</param>
        public static void ShowFailure(string message)
        {
            MessageBox.Show(message, "Failed!");
        }

        /// <summary>
        /// Shows a confirmation MessageBox
        /// </summary>
        /// <param name="message">The message to show</param>
        /// <returns></returns>
        public static MessageBoxResult ShowConfirm(string message)
        {
            return MessageBox.Show(message, "Confirm", MessageBoxButton.YesNo);
        }

        /// <summary>
        /// Shows a confirmation MessageBox
        /// </summary>
        /// <param name="title">The title of the window</param>
        /// <param name="message">The message to show</param>
        /// <returns></returns>
        public static MessageBoxResult ShowConfirm(string title, string message)
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo);
        }
    }
}
