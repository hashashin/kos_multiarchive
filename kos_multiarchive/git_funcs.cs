using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using UnityEngine;

namespace kos_multiarchive
{
    partial class kos_multiarchive
    {
        private void NewArch(string branchname)
        {
            foreach (string branch in _branchNameList)
            {
                if (branch != branchname)
                {
                    _repo.CreateBranch(branchname);
                    _repo.Checkout(branchname);
                    _inusebranch = branchname;
                    GetBranches();
                    break;
                }
            }
        }

        private void DelArch(string branchname)
        {
            try
            {
                _repo.Branches.Remove(branchname);
            }

            catch (LibGit2SharpException e)
            {
                ScreenMessages.PostScreenMessage(e.Message,
                    4f, ScreenMessageStyle.UPPER_CENTER);
            }
            GetBranches();
        }

        private void ChangeArch()
        {
            var newbranch = _branchNameList[_selectionGridInt];
            _repo.Checkout(newbranch);
            _inusebranch = newbranch;
        }

        private void RestoreOrig()
        {
            _repo.Checkout("master");
            _inusebranch = _repo.Head.Name;
        }

        private void GetBranches()
        {
            if (_repo.Branches != null) _branchList = _repo.Branches.ToList();
            _branchNameList = new List<string>();
            if (_branchList.Count > 0)
            {
                foreach (Branch branch in _branchList)
                {
                    _branchNameList.Add(branch.Name);
                }
            }
        }

        private void Commit(string commitmsg)
        {
            try
            {
                _repo.Commit(commitmsg);
            }
            catch (LibGit2SharpException e)
            {
                ScreenMessages.PostScreenMessage(e.Message,
                    4f, ScreenMessageStyle.UPPER_CENTER);
            }
            
        }
    }
}
