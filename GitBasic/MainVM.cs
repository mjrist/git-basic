using Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using System.Windows;
using System.IO;

namespace GitBasic
{
    public class MainVM : Observable
    {
        public Prop<string> WorkingDirectory { get; set; } = new Prop<string>();
        public ReactiveProp<Repository> Repo { get; set; }
        public ReactiveProp<string> RepositoryName { get; set; }
        public ReactiveProp<string> BranchName { get; set; }
        public HotKeyHelper HotKeyHelper { get; set; }

        public MainVM()
        {            
            Repo = new ReactiveProp<Repository>(UpdateRepository, WorkingDirectory);
            RepositoryName = new ReactiveProp<string>(() => { return (Repo.Value != null) ? Repo.Value.Info.WorkingDirectory.TrimEnd('\\').SubstringFromLast('\\') : string.Empty; }, Repo);

            // TODO: This needs to update on branch changes as well. Currently only watching Repo which changes on directory changes.
            BranchName = new ReactiveProp<string>(() => { return (Repo.Value != null) ? Repo.Value.Head.FriendlyName : string.Empty; }, Repo);

            WorkingDirectory.Value = Properties.Settings.Default.WorkingDirectory;
        }        

        private Repository UpdateRepository()
        {
            Repo.Value?.Dispose();
            string repoPath = Repository.Discover(WorkingDirectory.Value);
            return (repoPath != null) ? new Repository(repoPath) : null;
        }
    }
}
