using DSharpPlus.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DiscordBotOnDScharp.Services
{
    public class BadWordsService
    {
        private HashSet<string> _badWords =null!;

        public async Task LoadBadWordsAsync(string filePath)
        {
            try
            {
                // Чтение списка плохих слов из файла и сохранение их в HashSet
                var words = await File.ReadAllLinesAsync(filePath);

                _badWords = new HashSet<string>(words, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                // Обработка ошибки чтения файла или других исключений
                Console.WriteLine($"Ошибка загрузки плохих слов: {ex.Message}");
            }
        }

        public bool ContainsBadWord(string str)
        {
            if (_badWords == null)
            {
                // Обработка ситуации, когда список плохих слов ещё не загружен
                return false;
            }

            string profanityPattern = @"\b(бл|пзд|ебл|еба|ёб|хуй|..хуя|пизд|шлюх)\b";
            Regex regex = new(profanityPattern, RegexOptions.IgnoreCase);
            var word = str.Split(' ');
            foreach (var w in word)
            {
                if (_badWords.Contains(w) || regex.IsMatch(w))
                    return true;
            }
            return false;
        }
    }
}
