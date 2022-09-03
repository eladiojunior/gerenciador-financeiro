namespace GFin.Service
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstallerGFin = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerGFin = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerGFin
            // 
            this.serviceProcessInstallerGFin.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerGFin.Password = null;
            this.serviceProcessInstallerGFin.Username = null;
            // 
            // serviceInstallerGFin
            // 
            this.serviceInstallerGFin.Description = "Serviço que verifica as contas (despesas e receitas) fixas do mês corrente e as registram como mensal.";
            this.serviceInstallerGFin.DisplayName = "GFin.VerificadorContasFixas";
            this.serviceInstallerGFin.ServiceName = "ServiceVerificadorContasFixas";
            this.serviceInstallerGFin.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerGFin,
            this.serviceInstallerGFin});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerGFin;
        private System.ServiceProcess.ServiceInstaller serviceInstallerGFin;
    }
}