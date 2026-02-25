using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mugen
{

	public class PlayerFiles : IConfigPropertys
	{
		public string ConfigName {
			get {
				return string.Empty;
			}
		}

		public string cmd {
			get; protected set;
		}

		public string cns {
			get; protected set;
		}

		// state
		public string st {
			get; protected set;
		}

		public string st2 {
			get; protected set;
		}

		public string st3 {
			get; protected set;
		}

		public string st4 {
			get; protected set;
		}

		public string anim {
			get; protected set;
		}

		public string sound {
			get; protected set;
		}

		public string stcommon {
			get; protected set;
		}

		public string sprite {
			get; protected set;
		}

		public string pal1 {
			get; set;
		}

		public string pal2 {
			get; set;
		}

		public string pal3 {
			get; set;
		}

		public string pal4 {
			get; set;
		}

		public string pal5 {
			get; set;
		}

		public string pal6 {
			get; set;
		}

		public string pal7 {
			get; set;
		}

		public string pal8 {
			get; set;
		}

		public string pal9 {
			get; set;
		}

		public string pal10 {
			get; set;
		}

		public string pal11 {
			get; set;
		}

		public string pal12 {
			get; set;
		}

		public bool HasPal {
			get {
				return (!string.IsNullOrEmpty(pal1)) || (!string.IsNullOrEmpty(pal2)) || (!string.IsNullOrEmpty(pal3))
					|| (!string.IsNullOrEmpty(pal4)) || (!string.IsNullOrEmpty(pal5)) || (!string.IsNullOrEmpty(pal6))
					|| (!string.IsNullOrEmpty(pal7)) || (!string.IsNullOrEmpty(pal8)) || (!string.IsNullOrEmpty(pal9))
					|| (!string.IsNullOrEmpty(pal10)) || (!string.IsNullOrEmpty(pal11)) || (!string.IsNullOrEmpty(pal12));
			}
		}

		public string[] ToPalLocalPaths {
			get {
				string[] ret = null;
				int cnt = this.PalCount;
				if (cnt <= 0)
					return ret;
				ret = new string[cnt];
				int idx = 0;
				AddPal(ret, ref idx, pal1);
				AddPal(ret, ref idx, pal2);
				AddPal(ret, ref idx, pal3);
				AddPal(ret, ref idx, pal4);
				AddPal(ret, ref idx, pal5);
				AddPal(ret, ref idx, pal6);
				AddPal(ret, ref idx, pal7);
				AddPal(ret, ref idx, pal8);
				AddPal(ret, ref idx, pal9);
				AddPal(ret, ref idx, pal10);
				AddPal(ret, ref idx, pal11);
				AddPal(ret, ref idx, pal12);

				return ret;
			}
		}

		public int PalCount {
			get {
				int ret = 0;
				AddPalCnt(ref ret, pal1);
				AddPalCnt(ref ret, pal2);
				AddPalCnt(ref ret, pal3);
				AddPalCnt(ref ret, pal4);
				AddPalCnt(ref ret, pal5);
				AddPalCnt(ref ret, pal6);
				AddPalCnt(ref ret, pal7);
				AddPalCnt(ref ret, pal8);
				AddPalCnt(ref ret, pal9);
				AddPalCnt(ref ret, pal10);
				AddPalCnt(ref ret, pal11);
				AddPalCnt(ref ret, pal12);

				return ret;
			}
		}

		private static void AddPalCnt(ref int cnt, string pal) {
			if (!string.IsNullOrEmpty(pal))
				cnt += 1;
        }

		private static void AddPal(string[] arr, ref int idx, string pal) {
			if (arr == null || idx < 0 || idx >= arr.Length)
				return;
			arr[idx] = pal;
			idx += 1;
		}

		public string ai {
			get; protected set;
		}


		public bool IsDefaultPal {
			get;
			set;
		}
	}

	public class PlayerCfgLoader
	{
		public PlayerCfgLoader(string text) {
			LoadString(text);
		}

		private void LoadString(string str) {
			if (string.IsNullOrEmpty(str))
				return;

			ConfigReader reader = new ConfigReader();
			reader.LoadString(str);
			var section = reader.GetSection("Files");
			if (section == null)
				return;
			mPlayerFiles = new PlayerFiles();
			if (!section.GetPropertysValues(mPlayerFiles))
				mPlayerFiles = null;
		}

		public PlayerFiles files {
			get {
				return mPlayerFiles;
			}
		}

		private PlayerFiles mPlayerFiles = null;
	}

}