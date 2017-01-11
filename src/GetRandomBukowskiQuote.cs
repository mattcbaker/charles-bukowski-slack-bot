using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace CharlesBukowskiSlackBot
{
    interface IGetRandomBukowskiQuote
    {
        string GetNextQuote();
    }

    class GetRandomBukowskiQuote : IGetRandomBukowskiQuote
    {
        private string[] quotes = new[]
        {
            "Some people never go crazy. What truly horrible lives they must lead.",
            "I don't hate them...I just feel better when they're not around.",
            "For those who believe in God, most of the big questions are answered. But for those of us who can\'t readily accept the God formula, the big answers don\'t remain stone-written. We adjust to new conditions and discoveries. We are pliable. Love need not be a command nor faith a dictum. I am my own god. We are here to unlearn the teachings of the church, state, and our educational system. We are here to drink beer. We are here to kill war. We are here to laugh at the odds and live our lives so well that Death will tremble to take us.",
            "Sometimes you climb out of bed in the morning and you think, I\'m not going to make it, but you laugh inside - remembering all the times you\'ve felt that way.",
            "I\'ve never been lonely. I\'ve been in a room -- I\'ve felt suicidal. I\'ve been depressed. I\'ve felt awful -- awful beyond all -- but I never felt that one other person could enter that room and cure what was bothering me...or that any number of people could enter that room. In other words, loneliness is something I\'ve never been bothered with because I\'ve always had this terrible itch for solitude. It\'s being at a party, or at a stadium full of people cheering for something, that I might feel loneliness. I\'ll quote Ibsen, \"The strongest men are the most alone.\" I\'ve never thought, \"Well, some beautiful blonde will come in here and give me a fuck-job, rub my balls, and I\'ll feel good.\" No, that won\'t help. You know the typical crowd, \"Wow, it\'s Friday night, what are you going to do? Just sit there?\" Well, yeah. Because there\'s nothing out there. It\'s stupidity. Stupid people mingling with stupid people. Let them stupidify themselves. I\'ve never been bothered with the need to rush out into the night. I hid in bars, because I didn\'t want to hide in factories. That\'s all. Sorry for all the millions, but I\'ve never been lonely. I like myself. I\'m the best form of entertainment I have. Let\'s drink more wine!",
            "what matters most is how well you walk through the fire",
            "We\'re all going to die, all of us, what a circus! That alone should make us love each other but it doesn\'t. We are terrorized and flattened by trivialities, we are eaten up by nothing.",
            "If you\'re going to try, go all the way. Otherwise, don\'t even start. This could mean losing girlfriends, wives, relatives and maybe even your mind. It could mean not eating for three or four days. It could mean freezing on a park bench. It could mean jail. It could mean derision. It could mean mockery--isolation. Isolation is the gift. All the others are a test of your endurance, of how much you really want to do it. And, you\'ll do it, despite rejection and the worst odds. And it will be better than anything else you can imagine. If you\'re going to try, go all the way. There is no other feeling like that. You will be alone with the gods, and the nights will flame with fire. You will ride life straight to perfect laughter. It\'s the only good fight there is.",
            "My ambition is handicapped by laziness",
            "You have to die a few times before you can really live.",
            "My dear,\r\nFind what you love and let it kill you.\r\nLet it drain you of your all. Let it cling onto your back and weigh you down into eventual nothingness.\r\nLet it kill you and let it devour your remains.\r\nFor all things will kill you, both slowly and fastly, but it’s much better to be killed by a lover.\r\n~ Falsely yours",
            "That\'s the problem with drinking, I thought, as I poured myself a drink. If something bad happens you drink in an attempt to forget; if something good happens you drink in order to celebrate; and if nothing happens you drink to make something happen.",
            "The problem with the world is that the intelligent people are full of doubts, while the stupid ones are full of confidence.",
            "I wanted the whole world or nothing.",
            "there is a loneliness in this world so great\r\nthat you can see it in the slow movement of\r\nthe hands of a clock.\r\n\r\npeople so tired\r\nmutilated\r\neither by love or no love.\r\n\r\npeople just are not good to each other\r\none on one.\r\n\r\nthe rich are not good to the rich\r\nthe poor are not good to the poor.\r\n\r\nwe are afraid.\r\n\r\nour educational system tells us\r\nthat we can all be\r\nbig-ass winners.\r\n\r\nit hasn\'t told us\r\nabout the gutters\r\nor the suicides.\r\n\r\nor the terror of one person\r\naching in one place\r\nalone\r\n\r\nuntouched\r\nunspoken to\r\n\r\nwatering a plant.",
            "An intellectual says a simple thing in a hard way. An artist says a hard thing in a simple way.",
            "there are worse things\r\nthan being alone\r\nbut it often takes\r\ndecades to realize this\r\nand most often when you do\r\nit\'s too late\r\nand there\'s nothing worse\r\nthan too late",
            "If you're losing your soul and you know it, then you've still got a soul left to lose",
            "Find what you love and let it kill you.",
            "Real loneliness is not necessarily limited to when you are alone.",
            "Some lose all mind and become soul,insane.\r\nsome lose all soul and become mind, intellectual.\r\nsome lose both and become accepted",
            "I loved you like a man loves a woman he never touches, only writes to, keeps little photographs of.",
            "Being alone never felt right. sometimes it felt good, but it never felt right.",
            "I felt like crying but nothing came out. it was just a sort of sad sickness, sick sad, when you can\'t feel any worse. I think you know it. I think everybody knows it now and then. but I think I have known it pretty often, too often.",
            "I will remember the kisses \r\nour lips raw with love \r\nand how you gave me \r\neverything you had \r\nand how I \r\noffered you what was left of \r\nme, \r\nand I will remember your small room \r\nthe feel of you \r\nthe light in the window \r\nyour records \r\nyour books \r\nour morning coffee \r\nour noons our nights \r\nour bodies spilled together \r\nsleeping \r\nthe tiny flowing currents \r\nimmediate and forever \r\nyour leg my leg \r\nyour arm my arm \r\nyour smile and the warmth \r\nof you \r\nwho made me laugh \r\nagain.",
            "Boring damned people. All over the earth. Propagating more boring damned people. What a horror show. The earth swarmed with them.",
            "The free soul is rare, but you know it when you see it - basically because you feel good, very good, when you are near or with them.",
            "those who escape hell\r\nhowever\r\nnever talk about\r\nit\r\nand nothing much\r\nbothers them\r\nafter\r\nthat.",
            "A love like that was a serious illness, an illness from which you never entirely recover.",
            "There\'s a bluebird in my heart that wants to get out\r\nbut I\'m too tough for him,\r\nI say, stay in there, I\'m not going to let anybody see you."
        };

        private Queue<string> queue;

        public GetRandomBukowskiQuote()
        {
            var shuffled = Shuffle(this.quotes);
            queue = new Queue<string>(shuffled);
        }

        public string GetNextQuote()
        {
            var quote = queue.Dequeue();
            queue.Enqueue(quote);
            return quote;
        }

        private string[] Shuffle(string[] array)
        {
            var random = new Random();
            var n = array.Length;
            while (n > 1)
            {
                var indexToSwap = random.Next(n--);
                var swap = array[indexToSwap];
                array[indexToSwap] = array[n];
                array[n] = swap;
            }

            return array;
        }
    }
}