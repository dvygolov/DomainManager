﻿using System;

namespace DomainManager
{
    public static class YesNoSelector
    {
        public static bool ReadAnswerEqualsYes()
        {
            var answer = Console.ReadKey();
            Console.WriteLine();
            return answer.KeyChar == 'Y' || answer.KeyChar == 'y';
        }

        public static int GetMenuAnswer(int maxCount)
        {
            bool answerIsOk = false;
            int index=0;
            while (!answerIsOk)
            {
                Console.Write("Ваш выбор:");
                var answer = Console.ReadLine();
                if (!int.TryParse(answer, out _)) continue;
                index = int.Parse(answer);
                if (index < 0 || index > maxCount) continue;
                answerIsOk=true;
            }
            return index;
        }
    }
}
