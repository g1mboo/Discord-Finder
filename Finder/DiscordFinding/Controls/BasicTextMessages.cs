using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordFinding.Controls
{
    public struct BasicTextMessages
    {
        public static string IfSavedCrashed { get => "Изменеия не были применены, возможен не корректный ввод данных"; }        
        public static string IfNoChangesRequired { get => "Группы обеих версей(правленная и старая) идентичны!"; }        
        public static string IfEmptyField { get => "Все поля ввода должны быть заполнены"; }        
        public static string IfEmptyList { get => "Cписок групп пуст"; }        
        public static string IfEmptyItemTryChange { get => "Нельзя изменить то чего нет"; }        
        public static string IfEmptyItemTryModificate { get => "Невозможно добавить участника(-ов)"; }        
        public static string IfNoItemIsSelected { get => "Ни одного участника группы не выбрано"; }
        public static string IfNonUnicField { get => "Поля ввода должны быть уникальными"; }        
        public static string IfUserNonUnicField { get => "Данные участников группы не могут совпадать"; }        
        public static string IfSuccessfulShort { get => "Успешно. В силе после закрытия"; }        
        public static string IfSuccessfulUltraShort { get => "Изменения успешно применены. "; }        
        public static string IfNameChange { get => "Имя успешно изменено"; }        
        public static string IfChangesReset { get => "Измения сброшены"; }

    }
}
