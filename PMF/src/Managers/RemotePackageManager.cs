﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace PMF.Managers
{
    internal static class RemotePackageManager
    {
        /// <summary>
        /// Gets package info from the server along with ALL the assets in the json
        /// </summary>
        /// <param name="id">The id of the package</param>
        /// <returns>The package object downloaded</returns>
        public static Package GetPackageInfo(string id)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    PMF.InvokePackageMessageEvent("Downloading package information");
                    string json = client.DownloadString($"{Config.RepositoryEndpoint}/{id}");
                    PMF.InvokePackageMessageEvent("Parsing package information");
                    return JsonConvert.DeserializeObject<Package>(json);
                }
            }
            catch (WebException)
            {
                PMF.InvokePackageMessageEvent("Couldn't download information from the server");
                return null;
            }
        }

        /// <summary>
        /// Downloads a specific version of a certain package
        /// </summary>
        /// <param name="id">The id of the package</param>
        /// <param name="asset">The asset that is to be downloaded</param>
        /// <returns>The zip file which was downloaded</returns>
        public static string DownloadAsset(string id, Asset asset)
        {
            using (WebClient client = new WebClient())
            {
                PMF.InvokePackageMessageEvent("Downloading asset");

                var zipPath = Path.Combine(Config.TemporaryFolder, id);
                client.DownloadFile(asset.Url, Path.Combine(zipPath, asset.FileName));

                foreach (var dependency in asset.Dependencies)
                {
                    PMF.InvokePackageMessageEvent($"Downloading dependency with id: {dependency.ID}");
                    client.DownloadFile(dependency.Url, Path.Combine(zipPath, dependency.FileName));
                }

                PMF.InvokePackageMessageEvent("Finished downloading all required files");

                return zipPath;
            }
        }

        /// <summary>
        /// Gets you the latest version of a package
        /// </summary>
        /// <param name="package">The package object to get the latest version</param>
        /// <returns>The latest asset version of a given package</returns>
        public static Asset GetAssetLatestVersion(Package package)
        {
            if (package == null)
                throw new ArgumentNullException();

            if (package.Assets.Count == 0)
                return null;

            Asset ret_asset = null;
            foreach (var asset in package.Assets)
            {
                if (ret_asset == null || ret_asset.Version < asset.Version)
                    ret_asset = asset;
            }

            return ret_asset;
        }

        /// <summary>
        /// Gets you the latest version of a package given an SDK version
        /// </summary>
        /// <param name="package">The package object to get the asset</param>
        /// <returns>The latest asset version of a given package and given SDK version</returns>
        public static Asset GetAssetLatestVersionBySdkVersion(Package package)
        {
            if (package == null)
                throw new ArgumentNullException();
            if (package.Assets.Count == 0)
                return null;

            Asset ret_asset = null;
            foreach (var asset in package.Assets)
            {
                if (asset.SdkVersion == Config.CurrentSdkVersion)
                {
                    if (ret_asset == null || ret_asset.Version < asset.Version)
                        ret_asset = asset;
                }
            }

            return ret_asset;
        }
    }
}
