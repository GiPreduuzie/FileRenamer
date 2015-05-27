using System;
using System.IO;

namespace DecompiledMp3Lib.FileMover
{
    public static class FileMover
    {
        public static void FileMove(FileInfo sourceLocation, FileInfo targetLocation, FileInfo backupLocation)
        {
            if (targetLocation.Exists)
            {
                try
                {
                    File.Replace(sourceLocation.FullName, targetLocation.FullName, backupLocation.FullName, true);
                }
                catch (PlatformNotSupportedException ex)
                {
                    FileMover.NonNtfsReplace(sourceLocation, targetLocation, backupLocation);
                }
            }
            else
                File.Move(sourceLocation.FullName, targetLocation.FullName);
        }

        private static void NonNtfsReplace(FileInfo sourceLocation, FileInfo targetLocation, FileInfo backupLocation)
        {
            FileInfo fileInfo = (FileInfo)null;
            if (backupLocation.Exists)
            {
                fileInfo = new FileInfo(Path.Combine(Path.GetDirectoryName(backupLocation.FullName), Path.GetRandomFileName()));
                File.Move(backupLocation.FullName, fileInfo.FullName);
            }
            try
            {
                File.Move(targetLocation.FullName, backupLocation.FullName);
            }
            catch
            {
                if (fileInfo != null)
                    File.Move(fileInfo.FullName, backupLocation.FullName);
                throw;
            }
            try
            {
                File.Move(sourceLocation.FullName, targetLocation.FullName);
            }
            catch
            {
                File.Move(backupLocation.FullName, targetLocation.FullName);
                if (fileInfo != null)
                    File.Move(fileInfo.FullName, backupLocation.FullName);
                throw;
            }
            try
            {
                fileInfo.Delete();
            }
            catch
            {
            }
        }
    }
}
