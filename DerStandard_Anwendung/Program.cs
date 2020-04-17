using System;
using Magazine_Structure;

namespace DerStandard_Anwendung
{
    class Program
    {
        static void Main(string[] args)
        {        


            DerStandardAnalyse analyse = new DerStandardAnalyse();
            Magazine derStandard = new Magazine();

            derStandard.Add("international", "https://www.derstandard.at/international");
            derStandard.Add("inland", "https://www.derstandard.at/inland");
            derStandard.Add("wirtschaft", "https://www.derstandard.at/wirtschaft");
            derStandard.Add("web", "https://www.derstandard.at/web");
            derStandard.Add("sport", "https://www.derstandard.at/sport");
            derStandard.Add("panorama", "https://www.derstandard.at/panorama/");
            derStandard.Add("kultur", "https://www.derstandard.at/kultur");
            derStandard.Add("etat", "https://www.derstandard.at/etat", "s3");
            derStandard.Add("wissenschaft", "https://www.derstandard.at/wissenschaft", "s7");


            //derStandard.Add("lifestyle", "https://www.derstandard.at/lifestyle", "s6"); //lifestyle funkt ned weil bei  kreuzworträtseln des script von derStandard defekt is..... und bei rezepten oder so da article cicker ned funkt

            #region reicht scho..
            //derStandard.Add("diskurs", "https://www.derstandard.at/diskurs", "s10");
            //derStandard.Add("karriere", "https://www.derstandard.at/karriere", "s3");
            //derStandard.Add("immobilien", "https://www.derstandard.at/immobilien");
            //derStandard.Add("zukunft", "https://www.derstandard.at/zukunft");
            //derStandard.Add("gesundheit", "https://www.derstandard.at/gesundheit");
            //derStandard.Add("familie", "https://www.derstandard.at/lifestyle/familie");
            //derStandard.Add("bildung", "https://www.derstandard.at/inland/bildung");
            #endregion

            derStandard = analyse.AnalyzeData(derStandard);
            derStandard.ExportToTSV();
        }
    }
}
