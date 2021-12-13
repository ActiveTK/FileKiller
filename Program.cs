
/*
 * ファイル完全削除ツール ファイルキラー
 * (c) 2021 ActiveTK.
 * Released under the MIT License
 */

using System;
using System.IO;

namespace FileKiller
{
    class Call
    {
        /// <summary>
        /// システムによって呼び出される、メイン関数です。
        /// </summary>
        /// <param name="args[0]">(string) 削除したいファイルのパス。</param>
        /// <param name="args[1]">(int) 削除レベル。詳細はREADME.mdを参照してください。</param>
        /// <returns>正常終了は0。それ以外は失敗若しくは異常終了。</returns>
        static int Main(string[] args)
        {
            /*!**************************************************
             ** 以下のコードを編集/削除しないでください。
             ** Do NOT edit/remove this code!
             ****************************************************/
            Console.WriteLine("**********************************************************************");
            Console.WriteLine("** FileKiller - ファイル完全削除ツール / build 13 Dec, 2021");
            Console.WriteLine("** (c) 2021 ActiveTK. <+activetk.cf>");
            Console.WriteLine("** Released under the MIT License");
            Console.WriteLine("**********************************************************************");
            /*!**************************************************
             ****************************************************/
            var info = new RemoveInfo();
            if (args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("** arg[0] => (string)FilePath > ");
                Console.ResetColor();
                info.FilePath = Console.ReadLine();
                ViewAllLevel();
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("** arg[1] => (int)Level > ");
                    Console.ResetColor();
                    var level = Console.ReadLine();
                    if (int.TryParse(level, out info.RemoveLevel) && info.RemoveLevel < 6 && info.RemoveLevel > -1)
                        break;
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("** 上の一覧表を元に、0～5の数字を入力してください。");
                        Console.ResetColor();
                    }
                }
            }
            else if (args.Length == 1)
            {
                info.FilePath = args[0];
                ViewAllLevel();
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("** arg[1] => (int)Level > ");
                    Console.ResetColor();
                    var level = Console.ReadLine();
                    if (int.TryParse(level, out info.RemoveLevel) && info.RemoveLevel < 6 && info.RemoveLevel > -1)
                        break;
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("** 上の一覧表を元に、0～5の数字を入力してください。");
                        Console.ResetColor();
                    }
                }
            }
            else if (args.Length == 2)
            {
                info.FilePath = args[0];
                while (true)
                {
                    if (int.TryParse(args[1], out info.RemoveLevel) && info.RemoveLevel < 6 && info.RemoveLevel > -1)
                        break;
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("** args[1]には0～5の数字を入力してください。");
                        Console.ResetColor();
                        return -1;
                    }
                }
            }
            else
            {
                info.FilePath = args[0];
            }
            var killer = new RemoveFile(info);
            killer.Kill();
            if (killer.status != 0)
            {
                Console.WriteLine("終了するには何かキーを押してください。。");
                Console.ReadKey(false);
            }
            return killer.status;
        }
        private static void ViewAllLevel()
        {
            Console.WriteLine("**********************************************************************");
            Console.WriteLine("** 【削除レベル 一覧】");
            Console.WriteLine("** *******************************************************************");
            Console.WriteLine("** レベル **            説明              **        用途例");
            Console.WriteLine("** *******************************************************************");
            Console.WriteLine("**     0  ** 単純にファイルを削除         ** ゴミ箱を経由せずにファイルを削除");
            Console.WriteLine("**     1  ** 1回0x00で上書きして削除      ** SSDで復元されたくないファイルを削除");
            Console.WriteLine("**     2  ** 3回簡易乱数で上書きして削除  ** HDDで復元されたくないファイルを削除");
            Console.WriteLine("**     3  ** 3回疑似乱数で上書きして削除  ** 重要で機密のファイルを削除");
            Console.WriteLine("**     4  ** 10回疑似乱数で上書きして削除 ** 会社の存続に関わるのファイルを削除");
            Console.WriteLine("**     5  ** 50回疑似乱数で上書きして削除 ** 国家機密のファイルを削除");
            Console.WriteLine("**********************************************************************");
        }
    }
    public class RemoveFile
    {
        private string FilePath_ = "";
        private int Level = 0;
        public int status = 1;
        public RemoveFile(RemoveInfo info)
        {
            FilePath_ = info.FilePath;
            Level = info.RemoveLevel;
        }
        public void Kill()
        {
            if (Directory.Exists(FilePath_))
            {
                try
                {
                    foreach (var FilePath__ in Directory.GetFiles(FilePath_, "*", SearchOption.AllDirectories))
                        __kill(FilePath__);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e);
                    Console.ResetColor();
                    status = -1;
                }
                try
                {
                    Directory.Delete(FilePath_, true);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e);
                    Console.ResetColor();
                }
            }
            else
            {
                __kill(FilePath_);
            }
        }
        private void __kill(string FilePath)
        {
            try
            {
                Console.WriteLine("!* [Level" + Level + "] ファイル「" + FilePath + "」を削除しています。。");
                if (Level == 0)
                {
                    Console.WriteLine("** ファイルを削除しています。。");
                    File.Delete(FilePath);
                    Console.WriteLine("** ファイルを削除しました！");
                }
                else if (Level == 1)
                {
                    Console.WriteLine("** ファイルハンドラーを生成しています。。");
                    FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite);
                    fs.Position = 0;
                    Console.WriteLine("** ファイルを 0x00 で上書きしています。。");
                    long fslen = fs.Length;
                    byte[] zero = new byte[] { 0x00 };
                    byte[] zero1000 = new byte[1000];
                    for (int index = 0; index < 1000; index++)
                        zero1000[index] = 0x00;
                    byte[] zero10000 = new byte[10000];
                    for (int index = 0; index < 1000; index++)
                        zero10000[index] = 0x00;
                    int i = 0;
                    while (true)
                    {
                        if (fslen - i > 100000)
                        {
                            byte[] zero100000 = new byte[100000];
                            for (int index = 0; index < 1000; index++)
                                zero100000[index] = 0x00;
                            i += 100000;
                            fs.Write(zero100000, 0, 100000);
                            fs.Position = i;
                        }
                        else if (fslen - i > 10000)
                        {
                            i += 10000;
                            fs.Write(zero10000, 0, zero10000.Length);
                            fs.Position = i;
                        }
                        else if (fslen - i > 1000)
                        {
                            i += 1000;
                            fs.Write(zero1000, 0, zero1000.Length);
                            fs.Position = i;
                        }
                        else
                        {
                            i++;
                            fs.Write(zero, 0, zero.Length);
                            fs.Position = i;
                        }
                        try
                        {
                            Console.CursorLeft = 0;
                            Console.Write("[" +
                                Math.Round(decimal.Parse(i.ToString()) / decimal.Parse(fslen.ToString()) * 100, 2, MidpointRounding.AwayFromZero)
                                + "%完了] " + i + "/" + fslen + "      ");
                        }
                        catch { }
                        if (i == fslen)
                            break;
                    }
                    fs.Close();
                    Console.WriteLine();
                    Console.WriteLine("** ファイルを削除しています。。");
                    File.Delete(FilePath);
                    Console.WriteLine("** ファイルを削除しました！");
                }
                else if (Level == 2)
                {
                    Console.WriteLine("** ファイルハンドラーを生成しています。。");
                    FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite);
                    int s = 0;
                    int times = 3;
                    long fslen = fs.Length;
                    while (true)
                    {
                        s++;
                        fs.Position = 0;
                        if (s != 1)
                            Console.WriteLine();
                        Console.WriteLine("** [" + s + "回目] ファイルを簡易乱数で上書きしています。。");
                        long i = 0;
                        Random randx = new Random(new Random().Next(int.MinValue, int.MaxValue));
                        while (true)
                        {
                            if (fslen - i > 100000)
                            {
                                byte[] rand = new byte[100000];
                                randx.NextBytes(rand);
                                i += 100000;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            else if (fslen - i > 10000)
                            {
                                byte[] rand = new byte[10000];
                                randx.NextBytes(rand);
                                i += 10000;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            else if (fslen - i > 1000)
                            {
                                byte[] rand = new byte[1000];
                                randx.NextBytes(rand);
                                i += 1000;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            else
                            {
                                byte[] rand = new byte[1];
                                randx.NextBytes(rand);
                                i++;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            try
                            {
                                Console.CursorLeft = 0;
                                Console.Write("[" +
                                  Math.Round(decimal.Parse(i.ToString()) / decimal.Parse(fslen.ToString()) * 100, 2, MidpointRounding.AwayFromZero)
                                + "%完了] " + i + "/" + fslen + "      ");
                            }
                            catch { }
                            if (i == fslen)
                                break;
                        }
                        if (s == times)
                            break;
                    }
                    fs.Close();
                    Console.WriteLine();
                    Console.WriteLine("** ファイルを削除しています。。");
                    File.Delete(FilePath);
                    Console.WriteLine("** ファイルを削除しました！");
                }
                else if (Level == 3)
                {
                    Console.WriteLine("** ファイルハンドラーを生成しています。。");
                    FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite);
                    int s = 0;
                    int times = 3;
                    long fslen = fs.Length;
                    while (true)
                    {
                        s++;
                        fs.Position = 0;
                        if (s != 1)
                            Console.WriteLine();
                        Console.WriteLine("** [" + s + "回目] ファイルを疑似乱数で上書きしています。。");
                        long i = 0;

                        Console.WriteLine("** 疑似乱数のハンドラー(System.Security.Cryptography.RNGCryptoServiceProvider())を生成しています。。");
                        System.Security.Cryptography.RNGCryptoServiceProvider randx =
                                new System.Security.Cryptography.RNGCryptoServiceProvider();

                        while (true)
                        {
                            if (fslen - i > 100000)
                            {
                                byte[] rand = new byte[100000];
                                randx.GetBytes(rand);
                                i += 100000;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            else if (fslen - i > 10000)
                            {
                                byte[] rand = new byte[10000];
                                randx.GetBytes(rand);
                                i += 10000;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            else if (fslen - i > 1000)
                            {
                                byte[] rand = new byte[1000];
                                randx.GetBytes(rand);
                                i += 1000;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            else
                            {
                                byte[] rand = new byte[1];
                                randx.GetBytes(rand);
                                i++;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            try
                            {
                                Console.CursorLeft = 0;
                                Console.Write("[" +
                                  Math.Round(decimal.Parse(i.ToString()) / decimal.Parse(fslen.ToString()) * 100, 2, MidpointRounding.AwayFromZero)
                                + "%完了] " + i + "/" + fslen + "      ");
                            }
                            catch { }
                            if (i == fslen)
                                break;
                        }
                        randx.Dispose();
                        if (s == times)
                            break;
                    }
                    fs.Close();
                    Console.WriteLine();
                    Console.WriteLine("** ファイルを削除しています。。");
                    File.Delete(FilePath);
                    Console.WriteLine("** ファイルを削除しました！");
                }
                else if (Level == 4)
                {
                    Console.WriteLine("** ファイルハンドラーを生成しています。。");
                    FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite);
                    int s = 0;
                    int times = 10;
                    long fslen = fs.Length;
                    while (true)
                    {
                        s++;
                        fs.Position = 0;
                        if (s != 1)
                            Console.WriteLine();
                        Console.WriteLine("** [" + s + "回目] ファイルを疑似乱数で上書きしています。。");
                        long i = 0;

                        Console.WriteLine("** 疑似乱数のハンドラー(System.Security.Cryptography.RNGCryptoServiceProvider())を生成しています。。");
                        System.Security.Cryptography.RNGCryptoServiceProvider randx =
                                new System.Security.Cryptography.RNGCryptoServiceProvider();

                        while (true)
                        {
                            if (fslen - i > 100000)
                            {
                                byte[] rand = new byte[100000];
                                randx.GetBytes(rand);
                                i += 100000;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            else if (fslen - i > 10000)
                            {
                                byte[] rand = new byte[10000];
                                randx.GetBytes(rand);
                                i += 10000;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            else if (fslen - i > 1000)
                            {
                                byte[] rand = new byte[1000];
                                randx.GetBytes(rand);
                                i += 1000;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            else
                            {
                                byte[] rand = new byte[1];
                                randx.GetBytes(rand);
                                i++;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            try
                            {
                                Console.CursorLeft = 0;
                                Console.Write("[" +
                                  Math.Round(decimal.Parse(i.ToString()) / decimal.Parse(fslen.ToString()) * 100, 2, MidpointRounding.AwayFromZero)
                                + "%完了] " + i + "/" + fslen + "      ");
                            }
                            catch { }
                            if (i == fslen)
                                break;
                        }
                        randx.Dispose();
                        if (s == times)
                            break;
                    }
                    fs.Close();
                    Console.WriteLine();
                    Console.WriteLine("** ファイルを削除しています。。");
                    File.Delete(FilePath);
                    Console.WriteLine("** ファイルを削除しました！");
                }
                else if (Level == 5)
                {
                    Console.WriteLine("** ファイルハンドラーを生成しています。。");
                    FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite);
                    int s = 0;
                    int times = 50;
                    long fslen = fs.Length;
                    while (true)
                    {
                        s++;
                        fs.Position = 0;
                        if (s != 1)
                            Console.WriteLine();
                        Console.WriteLine("** [" + s + "回目] ファイルを疑似乱数で上書きしています。。");
                        long i = 0;

                        Console.WriteLine("** 疑似乱数のハンドラー(System.Security.Cryptography.RNGCryptoServiceProvider())を生成しています。。");
                        System.Security.Cryptography.RNGCryptoServiceProvider randx =
                                new System.Security.Cryptography.RNGCryptoServiceProvider();

                        while (true)
                        {
                            if (fslen - i > 100000)
                            {
                                byte[] rand = new byte[100000];
                                randx.GetBytes(rand);
                                i += 100000;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            else if (fslen - i > 10000)
                            {
                                byte[] rand = new byte[10000];
                                randx.GetBytes(rand);
                                i += 10000;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            else if (fslen - i > 1000)
                            {
                                byte[] rand = new byte[1000];
                                randx.GetBytes(rand);
                                i += 1000;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            else
                            {
                                byte[] rand = new byte[1];
                                randx.GetBytes(rand);
                                i++;
                                fs.Write(rand, 0, rand.Length);
                                fs.Position = i;
                            }
                            try
                            {
                                Console.CursorLeft = 0;
                                Console.Write("[" +
                                  Math.Round(decimal.Parse(i.ToString()) / decimal.Parse(fslen.ToString()) * 100, 2, MidpointRounding.AwayFromZero)
                                + "%完了] " + i + "/" + fslen + "      ");
                            }
                            catch { }
                            if (i == fslen)
                                break;
                        }
                        randx.Dispose();
                        if (s == times)
                            break;
                    }
                    fs.Close();
                    Console.WriteLine();
                    Console.WriteLine("** ファイルを削除しています。。");
                    File.Delete(FilePath);
                    Console.WriteLine("** ファイルを削除しました！");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("** エラー: 削除レベルが無効です。");
                    Console.ResetColor();
                    status = -1;
                    return;
                }
                status = 0;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ResetColor();
                status = -1;
            }
        }
    }
    public class RemoveInfo
    {
        public string FilePath = "";
        public int RemoveLevel = 0;
        public RemoveInfo()
        {

        }
    }
}
