using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitBasic.Controls
{
    /// <summary>
    /// Holds directory content matches for auto completion.
    /// </summary>
    public class AutoCompletion
    {
        public string GetNext(string token, string directory)
        {
            CheckIfReset(token, directory);

            if (_index == -1 || _index == _matches.Count - 1)
            {
                _index = 0;
            }
            else
            {
                _index++;
            }

            return _matches[_index];
        }

        public string GetPrevious(string token, string directory)
        {
            CheckIfReset(token, directory);

            if (_index == -1)
            {
                _index = 0;
            }
            else if (_index == 0)
            {
                _index = _matches.Count - 1;
            }
            else
            {
                _index--;
            }

            return _matches[_index];
        }

        private void CheckIfReset(string token, string directory)
        {
            if (token != _token || directory != _directory)
            {
                _token = token;
                _directory = directory;
                Reset();
            }
        }

        private void Reset()
        {
            _index = -1;

            string tokenPath = GetTokenPath();
            string directory = GetSearchDirectory(tokenPath);
            _matches = Directory.GetFileSystemEntries(directory).Select(x => tokenPath + Path.GetFileName(x)).ToList();

            if (!string.IsNullOrWhiteSpace(_token))
            {
                _matches = _matches
                    .Where(x => x.ToLower().StartsWith(_token.ToLower()) ||
                    x.ToLower().TrimStart('.').StartsWith(_token.ToLower())).ToList();
            }

            if (_matches.Count == 0)
            {
                _matches.Add(_token);
            }
        }

        private string GetTokenPath() => _token.Substring(0, _token.LastIndexOf('\\') + 1);

        private string GetSearchDirectory(string tokenPath)
        {
            string directory = Path.Combine(_directory, tokenPath);
            return (Directory.Exists(directory)) ? directory : _directory;            
        }

        private int _index = -1;
        private string _token = string.Empty;
        private string _directory = string.Empty;
        private List<string> _matches = new List<string>();
    }
}
