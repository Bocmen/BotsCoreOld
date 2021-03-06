using System;

namespace BotsCore.Moduls
{
    /// <summary>
    /// Модуль для логирования
    /// </summary>
    public static class EchoLog
    {
#pragma warning disable CA2211 // Поля, не являющиеся константами, не должны быть видимыми
        public static bool IsConsoleLog = true;
#pragma warning restore CA2211 // Поля, не являющиеся константами, не должны быть видимыми

        /// <summary>
        /// Залогировать событие
        /// </summary>
        /// <param name="TextLog">Текст лога</param>
        /// <param name="privilege">Приоритет события</param>
        public static void Print(string TextLog, string Path, PrivilegeLog privilege = PrivilegeLog.Info)
        {
            if (IsConsoleLog)
            {
                try
                {
                    switch (privilege)
                    {
                        case PrivilegeLog.Error:
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            break;
                        case PrivilegeLog.Info:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case PrivilegeLog.LogSystem:
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            break;
                        case PrivilegeLog.Warning:
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            break;
                    }
                    Console.WriteLine($"[{Path}] [{DateTime.Now.ToLocalTime()} {privilege}] {TextLog}");
                }
                catch { }
            }
        }
        public static void Print(string TextLog, PrivilegeLog privilege = PrivilegeLog.Info)
        {
            if (IsConsoleLog)
                Print(TextLog, new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.FullName.ToString(), privilege);
        }
        /// <summary>
        /// Виды привелегий
        /// </summary>
        public enum PrivilegeLog
        {
            /// <summary>
            /// Обычная информация
            /// </summary>
            Info,
            /// <summary>
            /// Системная информация
            /// </summary>
            LogSystem,
            /// <summary>
            /// Предупреждение
            /// </summary>
            Warning,
            /// <summary>
            /// Ошибка
            /// </summary>
            Error
        }
    }
}
