using LibGit2Sharp;
using Reactive;

namespace GitBasic
{
    public class MainVM : Observable
    {
        public Prop<string> WorkingDirectory { get; set; } = new Prop<string>();
        public Prop<string> SelectedFile { get; set; } = new Prop<string>();
        public ReactiveProp<Repository> Repo { get; set; }
        public ReactiveProp<string> RepositoryName { get; set; }
        public RepositoryChangeNotifier RepoNotifier { get; set; }
        public ReactiveProp<string> BranchName { get; set; }

        public MainVM()
        {
            Repo = new ReactiveProp<Repository>(UpdateRepository, WorkingDirectory);
            RepositoryName = new ReactiveProp<string>(() => { return (Repo.Value != null) ? Repo.Value.Info.WorkingDirectory.TrimEnd('\\').SubstringFromLast('\\') : string.Empty; }, Repo);
            RepoNotifier = new RepositoryChangeNotifier(Repo);
            BranchName = new ReactiveProp<string>(() => { return (Repo.Value != null) ? Repo.Value.Head.FriendlyName : string.Empty; }, Repo, RepoNotifier);
            CreateSubViewModels();

            // We set this property last after all the VMs have been set up.
            // As a dependency it triggers all the UI updates to run when set.
            WorkingDirectory.Value = Properties.Settings.Default.WorkingDirectory;
        }

        private Repository UpdateRepository()
        {
            Repo.Value?.Dispose();
            string repoPath = Repository.Discover(WorkingDirectory.Value);
            return (repoPath != null) ? new Repository(repoPath) : null;
        }

        // Sub View Models
        public CommandButtonVM CommandButtonVM { get; set; }
        public ConsoleControlVM ConsoleControlVM { get; set; }
        public FileStatusVM FileStatusVM { get; set; }
        public DiffViewerVM DiffViewerVM { get; set; }
        public RepositoryStatusBarVM RepositoryStatusBarVM { get; set; }

        private void CreateSubViewModels()
        {
            CommandButtonVM = new CommandButtonVM(this);
            ConsoleControlVM = new ConsoleControlVM(this);
            FileStatusVM = new FileStatusVM(this);
            DiffViewerVM = new DiffViewerVM(this);
            RepositoryStatusBarVM = new RepositoryStatusBarVM(this);
        }
    }
}
