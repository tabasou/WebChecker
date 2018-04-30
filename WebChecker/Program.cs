using System;
using System.IO;
using System.Text;  // for Encoding



namespace WebChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Wellcom to my home !!");
            WebPage webPage = new WebPage();
            webPage.outputHTMLfile();
            webPage.dispURL();

        }
    }
    #region WebPage_class
    class WebPage
    {
        String PageURL = "";
        const String URLfilename = "TargetURL.txt";
        #region Constracter
        /* コンストラクタ */
        public WebPage()
        {
            this.getPageURL();
        }
        public WebPage(String sPageURL)
        {
            this.PageURL = sPageURL;
        }
        #endregion
        #region method
        /* メソッド */
        void getPageURL()
        {
            /* ファイルからURLを読み込む */
            if (File.Exists(URLfilename))
            {
                StreamReader reader = new StreamReader(URLfilename,
                                           Encoding.GetEncoding("UTF-8"));
                string A;
                while ((A = reader.ReadLine()) != null)
                {
                    /* 読み込んだ１行分のテキスト A の処理 */
                    if (PageURL.Length == 0)
                    {
                        PageURL = A;
                    }
                }
                reader.Close();
            }
            else
            {  /* ファイルがなければ標準入力 */
                Console.WriteLine("Can't find file ! enter url");
                PageURL = Console.ReadLine();
            }
        }
        /* Debug用の標準出力 */
        public void dispURL()
        {
            Console.WriteLine(PageURL);
        }
        /* 取得したファイル内容を書きだす */
        public void outputHTMLfile(){
            String outputFilePath;
            String filePath;
            /* 出力Pathを作る */
            filePath = "~/WebCheckerLog/";
            filePath += midDomain();

            /*  */
            String fileName;
            fileName = createFileName();

            /*  */
            outputFilePath = System.IO.Path.Combine(filePath, fileName);

            Console.WriteLine(outputFilePath);




        }

        /* 出力するファイル名を返す */
        String createFileName(){
            String filename = "defaultfilename.html";

            DateTime now = DateTime.Now;
            filename = now.ToString("yyyyMMdd HHmmss") + ".html";

            return filename;
        }

        /* TargetURLからドメイン部分を切り出す */
        String midDomain(){
            String DomainName = "DomainName";
            String splitterStrt = "//";
            String splitterEnd = "/";
            int start;
            int end;

            start = PageURL.IndexOf(splitterStrt, StringComparison.Ordinal);
            if(start < 1){
                /* URLの中に"//"が無い場合はデフォルトの名前 */
                return DomainName;
            }
            end = PageURL.IndexOf(splitterEnd, start + splitterStrt.Length, StringComparison.Ordinal);

            if(end - start <= 0){
                return DomainName;
            }
            if(end - start > 30){
                /* 長い時は端折る */
                end = start + 30;
            }

            /* 切りだす */
            DomainName = PageURL.Substring(start+splitterStrt.Length, (end - (start + splitterStrt.Length)) + 1);

            return DomainName;
        }


        #endregion

    }
    #endregion
}
