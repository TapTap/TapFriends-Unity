using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LeanCloud.Storage;

namespace TapTap.GameSnapshot
{
    public class GameSnapshot : LCObject
    {
        public const string CLASS_NAME = "GameSnapshot";

        public string Name
        {
            get => this["name"] as string;
            set => this["name"] = value;
        }

        public string Description
        {
            get => this["description"] as string;
            set => this["description"] = value;
        }

        public DateTime ModifiedAt
        {
            get => this["modifiedAt"] is DateTime ? (DateTime) this["modifiedAt"] : default;
            set => this["modifiedAt"] = value;
        }

        public double PlayedTime
        {
            get => this["playedTime"] is double ? (double) this["playedTime"] : -1d;
            set => this["playedTime"] = value;
        }

        public int ProgressValue
        {
            get => this["progressValue"] is int ? (int) this["progressValue"] : -1;
            set => this["progressValue"] = value;
        }

        public LCFile Cover
        {
            get => this["cover"] as LCFile;
            set => this["cover"] = value;
        }

        public LCFile GameFile
        {
            get => this["gameFile"] as LCFile;
            set => this["gameFile"] = value;
        }

        public LCUser User
        {
            set => this["user"] = value;
        }

        public string CoverFilePath
        {
            set => this["cover"] = new LCFile("_cover", value);
        }

        public string GameFilePath
        {
            set => this["gameFile"] = new LCFile("_gameFile", value);
        }

        public GameSnapshot()
            : base(CLASS_NAME)
        {
        }

        public async Task<GameSnapshot> Save()
        {
            var currentUser = await LCUser.GetCurrent();
            if (currentUser == null) throw new UnauthorizedAccessException("Not Login");
            CheckArguments();
            var acl = new LCACL();
            acl.SetUserWriteAccess(currentUser, true);
            acl.SetUserReadAccess(currentUser, true);
            ACL = acl;
            User = currentUser;
            GameFile.ACL = acl;
            GameFile = await GameFile.Save();
            if (Cover != null)
            {
                Cover.ACL = acl;
                Cover = await Cover.Save();
            }

            return await base.Save() as GameSnapshot;
        }

        public static async Task<ReadOnlyCollection<GameSnapshot>> GetCurrentUserSnapshot()
        {
            var user = await LCUser.GetCurrent();
            if (user == null) throw new UnauthorizedAccessException("Not Login");
            return await ConstructorQueryByUser(user).Find();
        }

        public static LCQuery<GameSnapshot> GetQuery() => new LCQuery<GameSnapshot>(CLASS_NAME);

        private static LCQuery<GameSnapshot> ConstructorQueryByUser(LCUser user)
        {
            var query = GetQuery();
            query.Include("cover");
            query.Include("gameFile");
            query.WhereEqualTo("user", user);
            return query;
        }

        private void CheckArguments()
        {
            if (string.IsNullOrEmpty(Name)) throw new ArgumentNullException(nameof(Name));
            if (string.IsNullOrEmpty(Description)) throw new ArgumentNullException(nameof(Description));
            if (Description.Length > 1000) throw new ArgumentOutOfRangeException(nameof(Description));
            if (GameFile == null) throw new ArgumentNullException(nameof(GameFile));
            if (Cover == null) return;
            if (!GameSnapshotMimeType.SupportImageMimeType.Contains(Cover.MimeType))
                throw new ArgumentException("Cover File must be png or jpg");
        }

        private static class GameSnapshotMimeType
        {
            internal static readonly List<string> SupportImageMimeType = new List<string>
            {
                "image/png", "image/jpg"
            };
        }
    }
}