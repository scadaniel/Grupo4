; ModuleID = 'marshal_methods.arm64-v8a.ll'
source_filename = "marshal_methods.arm64-v8a.ll"
target datalayout = "e-m:e-i8:8:32-i16:16:32-i64:64-i128:128-n32:64-S128"
target triple = "aarch64-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [329 x ptr] zeroinitializer, align 8

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [987 x i64] [
	i64 u0x001e58127c546039, ; 0: lib_System.Globalization.dll.so => 42
	i64 u0x0024d0f62dee05bd, ; 1: Xamarin.KotlinX.Coroutines.Core.dll => 290
	i64 u0x0071cf2d27b7d61e, ; 2: lib_Xamarin.AndroidX.SwipeRefreshLayout.dll.so => 269
	i64 u0x01109b0e4d99e61f, ; 3: System.ComponentModel.Annotations.dll => 13
	i64 u0x02123411c4e01926, ; 4: lib_Xamarin.AndroidX.Navigation.Runtime.dll.so => 257
	i64 u0x0284512fad379f7e, ; 5: System.Runtime.Handles => 107
	i64 u0x02abedc11addc1ed, ; 6: lib_Mono.Android.Runtime.dll.so => 174
	i64 u0x02f55bf70672f5c8, ; 7: lib_System.IO.FileSystem.DriveInfo.dll.so => 48
	i64 u0x032267b2a94db371, ; 8: lib_Xamarin.AndroidX.AppCompat.dll.so => 211
	i64 u0x03621c804933a890, ; 9: System.Buffers => 7
	i64 u0x0363ac97a4cb84e6, ; 10: SQLitePCLRaw.provider.e_sqlite3.dll => 201
	i64 u0x0399610510a38a38, ; 11: lib_System.Private.DataContractSerialization.dll.so => 88
	i64 u0x043032f1d071fae0, ; 12: ru/Microsoft.Maui.Controls.resources => 318
	i64 u0x044440a55165631e, ; 13: lib-cs-Microsoft.Maui.Controls.resources.dll.so => 296
	i64 u0x046eb1581a80c6b0, ; 14: vi/Microsoft.Maui.Controls.resources => 324
	i64 u0x047408741db2431a, ; 15: Xamarin.AndroidX.DynamicAnimation => 231
	i64 u0x0517ef04e06e9f76, ; 16: System.Net.Primitives => 72
	i64 u0x0565d18c6da3de38, ; 17: Xamarin.AndroidX.RecyclerView => 261
	i64 u0x0581db89237110e9, ; 18: lib_System.Collections.dll.so => 12
	i64 u0x05989cb940b225a9, ; 19: Microsoft.Maui.dll => 194
	i64 u0x05a1c25e78e22d87, ; 20: lib_System.Runtime.CompilerServices.Unsafe.dll.so => 104
	i64 u0x06076b5d2b581f08, ; 21: zh-HK/Microsoft.Maui.Controls.resources => 325
	i64 u0x06388ffe9f6c161a, ; 22: System.Xml.Linq.dll => 159
	i64 u0x06600c4c124cb358, ; 23: System.Configuration.dll => 19
	i64 u0x067f95c5ddab55b3, ; 24: lib_Xamarin.AndroidX.Fragment.Ktx.dll.so => 236
	i64 u0x0680a433c781bb3d, ; 25: Xamarin.AndroidX.Collection.Jvm => 218
	i64 u0x069fff96ec92a91d, ; 26: System.Xml.XPath.dll => 164
	i64 u0x070b0847e18dab68, ; 27: Xamarin.AndroidX.Emoji2.ViewsHelper.dll => 233
	i64 u0x0728d149f3097b03, ; 28: App_v3 => 0
	i64 u0x0739448d84d3b016, ; 29: lib_Xamarin.AndroidX.VectorDrawable.dll.so => 273
	i64 u0x07469f2eecce9e85, ; 30: mscorlib.dll => 170
	i64 u0x07c57877c7ba78ad, ; 31: ru/Microsoft.Maui.Controls.resources.dll => 318
	i64 u0x07dcdc7460a0c5e4, ; 32: System.Collections.NonGeneric => 10
	i64 u0x08122e52765333c8, ; 33: lib_Microsoft.Extensions.Logging.Debug.dll.so => 189
	i64 u0x088610fc2509f69e, ; 34: lib_Xamarin.AndroidX.VectorDrawable.Animated.dll.so => 274
	i64 u0x08a7c865576bbde7, ; 35: System.Reflection.Primitives => 98
	i64 u0x08c9d051a4a817e5, ; 36: Xamarin.AndroidX.CustomView.PoolingContainer.dll => 229
	i64 u0x08f3c9788ee2153c, ; 37: Xamarin.AndroidX.DrawerLayout => 230
	i64 u0x09138715c92dba90, ; 38: lib_System.ComponentModel.Annotations.dll.so => 13
	i64 u0x0919c28b89381a0b, ; 39: lib_Microsoft.Extensions.Options.dll.so => 190
	i64 u0x092266563089ae3e, ; 40: lib_System.Collections.NonGeneric.dll.so => 10
	i64 u0x09d144a7e214d457, ; 41: System.Security.Cryptography => 129
	i64 u0x09e2b9f743db21a8, ; 42: lib_System.Reflection.Metadata.dll.so => 97
	i64 u0x0abb3e2b271edc45, ; 43: System.Threading.Channels.dll => 143
	i64 u0x0b06b1feab070143, ; 44: System.Formats.Tar => 39
	i64 u0x0b3b632c3bbee20c, ; 45: sk/Microsoft.Maui.Controls.resources => 319
	i64 u0x0b6aff547b84fbe9, ; 46: Xamarin.KotlinX.Serialization.Core.Jvm => 293
	i64 u0x0be2e1f8ce4064ed, ; 47: Xamarin.AndroidX.ViewPager => 276
	i64 u0x0c3ca6cc978e2aae, ; 48: pt-BR/Microsoft.Maui.Controls.resources => 315
	i64 u0x0c59ad9fbbd43abe, ; 49: Mono.Android => 175
	i64 u0x0c65741e86371ee3, ; 50: lib_Xamarin.Android.Glide.GifDecoder.dll.so => 205
	i64 u0x0c74af560004e816, ; 51: Microsoft.Win32.Registry.dll => 5
	i64 u0x0c7790f60165fc06, ; 52: lib_Microsoft.Maui.Essentials.dll.so => 195
	i64 u0x0c83c82812e96127, ; 53: lib_System.Net.Mail.dll.so => 68
	i64 u0x0cce4bce83380b7f, ; 54: Xamarin.AndroidX.Security.SecurityCrypto => 266
	i64 u0x0d13cd7cce4284e4, ; 55: System.Security.SecureString => 132
	i64 u0x0d63f4f73521c24f, ; 56: lib_Xamarin.AndroidX.SavedState.SavedState.Ktx.dll.so => 265
	i64 u0x0e04e702012f8463, ; 57: Xamarin.AndroidX.Emoji2 => 232
	i64 u0x0e14e73a54dda68e, ; 58: lib_System.Net.NameResolution.dll.so => 69
	i64 u0x0f37dd7a62ae99af, ; 59: lib_Xamarin.AndroidX.Collection.Ktx.dll.so => 219
	i64 u0x0f5e7abaa7cf470a, ; 60: System.Net.HttpListener => 67
	i64 u0x1001f97bbe242e64, ; 61: System.IO.UnmanagedMemoryStream => 57
	i64 u0x102a31b45304b1da, ; 62: Xamarin.AndroidX.CustomView => 228
	i64 u0x1065c4cb554c3d75, ; 63: System.IO.IsolatedStorage.dll => 52
	i64 u0x10f6cfcbcf801616, ; 64: System.IO.Compression.Brotli => 43
	i64 u0x114443cdcf2091f1, ; 65: System.Security.Cryptography.Primitives => 127
	i64 u0x11a603952763e1d4, ; 66: System.Net.Mail => 68
	i64 u0x11a70d0e1009fb11, ; 67: System.Net.WebSockets.dll => 83
	i64 u0x11f26371eee0d3c1, ; 68: lib_Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll.so => 246
	i64 u0x12128b3f59302d47, ; 69: lib_System.Xml.Serialization.dll.so => 161
	i64 u0x123639456fb056da, ; 70: System.Reflection.Emit.Lightweight.dll => 94
	i64 u0x12521e9764603eaa, ; 71: lib_System.Resources.Reader.dll.so => 101
	i64 u0x125b7f94acb989db, ; 72: Xamarin.AndroidX.RecyclerView.dll => 261
	i64 u0x12d3b63863d4ab0b, ; 73: lib_System.Threading.Overlapped.dll.so => 144
	i64 u0x134eab1061c395ee, ; 74: System.Transactions => 154
	i64 u0x138567fa954faa55, ; 75: Xamarin.AndroidX.Browser => 215
	i64 u0x13a01de0cbc3f06c, ; 76: lib-fr-Microsoft.Maui.Controls.resources.dll.so => 302
	i64 u0x13beedefb0e28a45, ; 77: lib_System.Xml.XmlDocument.dll.so => 165
	i64 u0x13f1e5e209e91af4, ; 78: lib_Java.Interop.dll.so => 172
	i64 u0x13f1e880c25d96d1, ; 79: he/Microsoft.Maui.Controls.resources => 303
	i64 u0x143d8ea60a6a4011, ; 80: Microsoft.Extensions.DependencyInjection.Abstractions => 183
	i64 u0x1497051b917530bd, ; 81: lib_System.Net.WebSockets.dll.so => 83
	i64 u0x14d612a531c79c05, ; 82: Xamarin.JSpecify.dll => 287
	i64 u0x14e68447938213b7, ; 83: Xamarin.AndroidX.Collection.Ktx.dll => 219
	i64 u0x152a448bd1e745a7, ; 84: Microsoft.Win32.Primitives => 4
	i64 u0x1557de0138c445f4, ; 85: lib_Microsoft.Win32.Registry.dll.so => 5
	i64 u0x15bdc156ed462f2f, ; 86: lib_System.IO.FileSystem.dll.so => 51
	i64 u0x15e300c2c1668655, ; 87: System.Resources.Writer.dll => 103
	i64 u0x16bf2a22df043a09, ; 88: System.IO.Pipes.dll => 56
	i64 u0x16ea2b318ad2d830, ; 89: System.Security.Cryptography.Algorithms => 122
	i64 u0x16eeae54c7ebcc08, ; 90: System.Reflection.dll => 100
	i64 u0x17125c9a85b4929f, ; 91: lib_netstandard.dll.so => 171
	i64 u0x1716866f7416792e, ; 92: lib_System.Security.AccessControl.dll.so => 120
	i64 u0x174f71c46216e44a, ; 93: Xamarin.KotlinX.Coroutines.Core => 290
	i64 u0x1752c12f1e1fc00c, ; 94: System.Core => 21
	i64 u0x17b56e25558a5d36, ; 95: lib-hu-Microsoft.Maui.Controls.resources.dll.so => 306
	i64 u0x17f9358913beb16a, ; 96: System.Text.Encodings.Web => 139
	i64 u0x1809fb23f29ba44a, ; 97: lib_System.Reflection.TypeExtensions.dll.so => 99
	i64 u0x18402a709e357f3b, ; 98: lib_Xamarin.KotlinX.Serialization.Core.Jvm.dll.so => 293
	i64 u0x18a9befae51bb361, ; 99: System.Net.WebClient => 79
	i64 u0x18f0ce884e87d89a, ; 100: nb/Microsoft.Maui.Controls.resources.dll => 312
	i64 u0x19777fba3c41b398, ; 101: Xamarin.AndroidX.Startup.StartupRuntime.dll => 268
	i64 u0x19a4c090f14ebb66, ; 102: System.Security.Claims => 121
	i64 u0x1a91866a319e9259, ; 103: lib_System.Collections.Concurrent.dll.so => 8
	i64 u0x1aac34d1917ba5d3, ; 104: lib_System.dll.so => 168
	i64 u0x1aad60783ffa3e5b, ; 105: lib-th-Microsoft.Maui.Controls.resources.dll.so => 321
	i64 u0x1aea8f1c3b282172, ; 106: lib_System.Net.Ping.dll.so => 71
	i64 u0x1b4b7a1d0d265fa2, ; 107: Xamarin.Android.Glide.DiskLruCache => 204
	i64 u0x1bbdb16cfa73e785, ; 108: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android => 247
	i64 u0x1bc766e07b2b4241, ; 109: Xamarin.AndroidX.ResourceInspection.Annotation.dll => 262
	i64 u0x1c753b5ff15bce1b, ; 110: Mono.Android.Runtime.dll => 174
	i64 u0x1cd47467799d8250, ; 111: System.Threading.Tasks.dll => 148
	i64 u0x1d23eafdc6dc346c, ; 112: System.Globalization.Calendars.dll => 40
	i64 u0x1da4110562816681, ; 113: Xamarin.AndroidX.Security.SecurityCrypto.dll => 266
	i64 u0x1db6820994506bf5, ; 114: System.IO.FileSystem.AccessControl.dll => 47
	i64 u0x1dbb0c2c6a999acb, ; 115: System.Diagnostics.StackTrace => 30
	i64 u0x1e3d87657e9659bc, ; 116: Xamarin.AndroidX.Navigation.UI => 259
	i64 u0x1e71143913d56c10, ; 117: lib-ko-Microsoft.Maui.Controls.resources.dll.so => 310
	i64 u0x1e7c31185e2fb266, ; 118: lib_System.Threading.Tasks.Parallel.dll.so => 147
	i64 u0x1ed8fcce5e9b50a0, ; 119: Microsoft.Extensions.Options.dll => 190
	i64 u0x1f055d15d807e1b2, ; 120: System.Xml.XmlSerializer => 166
	i64 u0x1f1ed22c1085f044, ; 121: lib_System.Diagnostics.FileVersionInfo.dll.so => 28
	i64 u0x1f61df9c5b94d2c1, ; 122: lib_System.Numerics.dll.so => 86
	i64 u0x1f750bb5421397de, ; 123: lib_Xamarin.AndroidX.Tracing.Tracing.dll.so => 270
	i64 u0x20237ea48006d7a8, ; 124: lib_System.Net.WebClient.dll.so => 79
	i64 u0x209375905fcc1bad, ; 125: lib_System.IO.Compression.Brotli.dll.so => 43
	i64 u0x20fab3cf2dfbc8df, ; 126: lib_System.Diagnostics.Process.dll.so => 29
	i64 u0x2110167c128cba15, ; 127: System.Globalization => 42
	i64 u0x21419508838f7547, ; 128: System.Runtime.CompilerServices.VisualC => 105
	i64 u0x2174319c0d835bc9, ; 129: System.Runtime => 119
	i64 u0x2198e5bc8b7153fa, ; 130: Xamarin.AndroidX.Annotation.Experimental.dll => 209
	i64 u0x219ea1b751a4dee4, ; 131: lib_System.IO.Compression.ZipFile.dll.so => 45
	i64 u0x21cc7e445dcd5469, ; 132: System.Reflection.Emit.ILGeneration => 93
	i64 u0x220fd4f2e7c48170, ; 133: th/Microsoft.Maui.Controls.resources => 321
	i64 u0x224538d85ed15a82, ; 134: System.IO.Pipes => 56
	i64 u0x22908438c6bed1af, ; 135: lib_System.Threading.Timer.dll.so => 151
	i64 u0x237be844f1f812c7, ; 136: System.Threading.Thread.dll => 149
	i64 u0x23852b3bdc9f7096, ; 137: System.Resources.ResourceManager => 102
	i64 u0x23986dd7e5d4fc01, ; 138: System.IO.FileSystem.Primitives.dll => 49
	i64 u0x2407aef2bbe8fadf, ; 139: System.Console => 20
	i64 u0x240abe014b27e7d3, ; 140: Xamarin.AndroidX.Core.dll => 224
	i64 u0x247619fe4413f8bf, ; 141: System.Runtime.Serialization.Primitives.dll => 116
	i64 u0x24de8d301281575e, ; 142: Xamarin.Android.Glide => 202
	i64 u0x252073cc3caa62c2, ; 143: fr/Microsoft.Maui.Controls.resources.dll => 302
	i64 u0x256b8d41255f01b1, ; 144: Xamarin.Google.Crypto.Tink.Android => 283
	i64 u0x25a0a7eff76ea08e, ; 145: SQLitePCLRaw.batteries_v2.dll => 198
	i64 u0x2662c629b96b0b30, ; 146: lib_Xamarin.Kotlin.StdLib.dll.so => 288
	i64 u0x268c1439f13bcc29, ; 147: lib_Microsoft.Extensions.Primitives.dll.so => 191
	i64 u0x26a670e154a9c54b, ; 148: System.Reflection.Extensions.dll => 96
	i64 u0x26d077d9678fe34f, ; 149: System.IO.dll => 58
	i64 u0x273f3515de5faf0d, ; 150: id/Microsoft.Maui.Controls.resources.dll => 307
	i64 u0x2742545f9094896d, ; 151: hr/Microsoft.Maui.Controls.resources => 305
	i64 u0x274d85d83ad40513, ; 152: lib_Xamarin.AndroidX.Window.WindowCore.dll.so => 279
	i64 u0x2759af78ab94d39b, ; 153: System.Net.WebSockets => 83
	i64 u0x27b2b16f3e9de038, ; 154: Xamarin.Google.Crypto.Tink.Android.dll => 283
	i64 u0x27b410442fad6cf1, ; 155: Java.Interop.dll => 172
	i64 u0x27b97e0d52c3034a, ; 156: System.Diagnostics.Debug => 26
	i64 u0x2801845a2c71fbfb, ; 157: System.Net.Primitives.dll => 72
	i64 u0x286835e259162700, ; 158: lib_Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll.so => 260
	i64 u0x28e52865585a1ebe, ; 159: Microsoft.Extensions.Diagnostics.Abstractions.dll => 184
	i64 u0x2949f3617a02c6b2, ; 160: Xamarin.AndroidX.ExifInterface => 234
	i64 u0x29aeab763a527e52, ; 161: lib_Xamarin.AndroidX.Navigation.Common.Android.dll.so => 255
	i64 u0x2a128783efe70ba0, ; 162: uk/Microsoft.Maui.Controls.resources.dll => 323
	i64 u0x2a3b095612184159, ; 163: lib_System.Net.NetworkInformation.dll.so => 70
	i64 u0x2a6507a5ffabdf28, ; 164: System.Diagnostics.TraceSource.dll => 33
	i64 u0x2ad156c8e1354139, ; 165: fi/Microsoft.Maui.Controls.resources => 301
	i64 u0x2ad5d6b13b7a3e04, ; 166: System.ComponentModel.DataAnnotations.dll => 14
	i64 u0x2af298f63581d886, ; 167: System.Text.RegularExpressions.dll => 141
	i64 u0x2afc1c4f898552ee, ; 168: lib_System.Formats.Asn1.dll.so => 38
	i64 u0x2b148910ed40fbf9, ; 169: zh-Hant/Microsoft.Maui.Controls.resources.dll => 327
	i64 u0x2b6989d78cba9a15, ; 170: Xamarin.AndroidX.Concurrent.Futures.dll => 220
	i64 u0x2c8bd14bb93a7d82, ; 171: lib-pl-Microsoft.Maui.Controls.resources.dll.so => 314
	i64 u0x2cbd9262ca785540, ; 172: lib_System.Text.Encoding.CodePages.dll.so => 136
	i64 u0x2cc9e1fed6257257, ; 173: lib_System.Reflection.Emit.Lightweight.dll.so => 94
	i64 u0x2cd723e9fe623c7c, ; 174: lib_System.Private.Xml.Linq.dll.so => 90
	i64 u0x2d169d318a968379, ; 175: System.Threading.dll => 152
	i64 u0x2d20145f27cfc1d2, ; 176: Xamarin.AndroidX.Window.WindowCore.Jvm.dll => 280
	i64 u0x2d47774b7d993f59, ; 177: sv/Microsoft.Maui.Controls.resources.dll => 320
	i64 u0x2d5ffcae1ad0aaca, ; 178: System.Data.dll => 24
	i64 u0x2db915caf23548d2, ; 179: System.Text.Json.dll => 140
	i64 u0x2dcaa0bb15a4117a, ; 180: System.IO.UnmanagedMemoryStream.dll => 57
	i64 u0x2e2ced2c3c6a1edc, ; 181: lib_System.Threading.AccessControl.dll.so => 142
	i64 u0x2e5a40c319acb800, ; 182: System.IO.FileSystem => 51
	i64 u0x2e6f1f226821322a, ; 183: el/Microsoft.Maui.Controls.resources.dll => 299
	i64 u0x2f02f94df3200fe5, ; 184: System.Diagnostics.Process => 29
	i64 u0x2f2e98e1c89b1aff, ; 185: System.Xml.ReaderWriter => 160
	i64 u0x2f5911d9ba814e4e, ; 186: System.Diagnostics.Tracing => 34
	i64 u0x2f84070a459bc31f, ; 187: lib_System.Xml.dll.so => 167
	i64 u0x309ee9eeec09a71e, ; 188: lib_Xamarin.AndroidX.Fragment.dll.so => 235
	i64 u0x30c6dda129408828, ; 189: System.IO.IsolatedStorage => 52
	i64 u0x31195fef5d8fb552, ; 190: _Microsoft.Android.Resource.Designer.dll => 328
	i64 u0x312c8ed623cbfc8d, ; 191: Xamarin.AndroidX.Window.dll => 278
	i64 u0x31496b779ed0663d, ; 192: lib_System.Reflection.DispatchProxy.dll.so => 92
	i64 u0x32243413e774362a, ; 193: Xamarin.AndroidX.CardView.dll => 216
	i64 u0x3235427f8d12dae1, ; 194: lib_System.Drawing.Primitives.dll.so => 35
	i64 u0x329753a17a517811, ; 195: fr/Microsoft.Maui.Controls.resources => 302
	i64 u0x32aa989ff07a84ff, ; 196: lib_System.Xml.ReaderWriter.dll.so => 160
	i64 u0x33829542f112d59b, ; 197: System.Collections.Immutable => 9
	i64 u0x33a31443733849fe, ; 198: lib-es-Microsoft.Maui.Controls.resources.dll.so => 300
	i64 u0x341abc357fbb4ebf, ; 199: lib_System.Net.Sockets.dll.so => 78
	i64 u0x346a212343615ac5, ; 200: lib_System.Linq.AsyncEnumerable.dll.so => 59
	i64 u0x3496c1e2dcaf5ecc, ; 201: lib_System.IO.Pipes.AccessControl.dll.so => 55
	i64 u0x34dfd74fe2afcf37, ; 202: Microsoft.Maui => 194
	i64 u0x34e292762d9615df, ; 203: cs/Microsoft.Maui.Controls.resources.dll => 296
	i64 u0x3508234247f48404, ; 204: Microsoft.Maui.Controls => 192
	i64 u0x353590da528c9d22, ; 205: System.ComponentModel.Annotations => 13
	i64 u0x3549870798b4cd30, ; 206: lib_Xamarin.AndroidX.ViewPager2.dll.so => 277
	i64 u0x355282fc1c909694, ; 207: Microsoft.Extensions.Configuration => 180
	i64 u0x3552fc5d578f0fbf, ; 208: Xamarin.AndroidX.Arch.Core.Common => 213
	i64 u0x355c649948d55d97, ; 209: lib_System.Runtime.Intrinsics.dll.so => 111
	i64 u0x35ea9d1c6834bc8c, ; 210: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll => 250
	i64 u0x3628ab68db23a01a, ; 211: lib_System.Diagnostics.Tools.dll.so => 32
	i64 u0x3673b042508f5b6b, ; 212: lib_System.Runtime.Extensions.dll.so => 106
	i64 u0x36740f1a8ecdc6c4, ; 213: System.Numerics => 86
	i64 u0x36b2b50fdf589ae2, ; 214: System.Reflection.Emit.Lightweight => 94
	i64 u0x36cada77dc79928b, ; 215: System.IO.MemoryMappedFiles => 53
	i64 u0x374ef46b06791af6, ; 216: System.Reflection.Primitives.dll => 98
	i64 u0x376bf93e521a5417, ; 217: lib_Xamarin.Jetbrains.Annotations.dll.so => 286
	i64 u0x37bc29f3183003b6, ; 218: lib_System.IO.dll.so => 58
	i64 u0x380134e03b1e160a, ; 219: System.Collections.Immutable.dll => 9
	i64 u0x38049b5c59b39324, ; 220: System.Runtime.CompilerServices.Unsafe => 104
	i64 u0x385c17636bb6fe6e, ; 221: Xamarin.AndroidX.CustomView.dll => 228
	i64 u0x38869c811d74050e, ; 222: System.Net.NameResolution.dll => 69
	i64 u0x393c226616977fdb, ; 223: lib_Xamarin.AndroidX.ViewPager.dll.so => 276
	i64 u0x395e37c3334cf82a, ; 224: lib-ca-Microsoft.Maui.Controls.resources.dll.so => 295
	i64 u0x39c3107c28752af1, ; 225: lib_Microsoft.Extensions.FileProviders.Abstractions.dll.so => 185
	i64 u0x3ab5859054645f72, ; 226: System.Security.Cryptography.Primitives.dll => 127
	i64 u0x3ad75090c3fac0e9, ; 227: lib_Xamarin.AndroidX.ResourceInspection.Annotation.dll.so => 262
	i64 u0x3ae44ac43a1fbdbb, ; 228: System.Runtime.Serialization => 118
	i64 u0x3b860f9932505633, ; 229: lib_System.Text.Encoding.Extensions.dll.so => 137
	i64 u0x3be99b43dd39dd37, ; 230: Xamarin.AndroidX.SavedState.SavedState.Android => 264
	i64 u0x3c3aafb6b3a00bf6, ; 231: lib_System.Security.Cryptography.X509Certificates.dll.so => 128
	i64 u0x3c4049146b59aa90, ; 232: System.Runtime.InteropServices.JavaScript => 108
	i64 u0x3c7c495f58ac5ee9, ; 233: Xamarin.Kotlin.StdLib => 288
	i64 u0x3c7e5ed3d5db71bb, ; 234: System.Security => 133
	i64 u0x3cd9d281d402eb9b, ; 235: Xamarin.AndroidX.Browser.dll => 215
	i64 u0x3d1c50cc001a991e, ; 236: Xamarin.Google.Guava.ListenableFuture.dll => 285
	i64 u0x3d2b1913edfc08d7, ; 237: lib_System.Threading.ThreadPool.dll.so => 150
	i64 u0x3d46f0b995082740, ; 238: System.Xml.Linq => 159
	i64 u0x3d8a8f400514a790, ; 239: Xamarin.AndroidX.Fragment.Ktx.dll => 236
	i64 u0x3d9c2a242b040a50, ; 240: lib_Xamarin.AndroidX.Core.dll.so => 224
	i64 u0x3da7781d6333a8fe, ; 241: SQLitePCLRaw.batteries_v2 => 198
	i64 u0x3dbb6b9f5ab90fa7, ; 242: lib_Xamarin.AndroidX.DynamicAnimation.dll.so => 231
	i64 u0x3e5441657549b213, ; 243: Xamarin.AndroidX.ResourceInspection.Annotation => 262
	i64 u0x3e57d4d195c53c2e, ; 244: System.Reflection.TypeExtensions => 99
	i64 u0x3e616ab4ed1f3f15, ; 245: lib_System.Data.dll.so => 24
	i64 u0x3f1d226e6e06db7e, ; 246: Xamarin.AndroidX.SlidingPaneLayout.dll => 267
	i64 u0x3f510adf788828dd, ; 247: System.Threading.Tasks.Extensions => 146
	i64 u0x3f6f5914291cdcf7, ; 248: Microsoft.Extensions.Hosting.Abstractions => 186
	i64 u0x407a10bb4bf95829, ; 249: lib_Xamarin.AndroidX.Navigation.Common.dll.so => 254
	i64 u0x40c98b6bd77346d4, ; 250: Microsoft.VisualBasic.dll => 3
	i64 u0x41833cf766d27d96, ; 251: mscorlib => 170
	i64 u0x41cab042be111c34, ; 252: lib_Xamarin.AndroidX.AppCompat.AppCompatResources.dll.so => 212
	i64 u0x423a9ecc4d905a88, ; 253: lib_System.Resources.ResourceManager.dll.so => 102
	i64 u0x423bf51ae7def810, ; 254: System.Xml.XPath => 164
	i64 u0x42462ff15ddba223, ; 255: System.Resources.Reader.dll => 101
	i64 u0x4291015ff4e5ef71, ; 256: Xamarin.AndroidX.Core.ViewTree.dll => 226
	i64 u0x42a31b86e6ccc3f0, ; 257: System.Diagnostics.Contracts => 25
	i64 u0x430e95b891249788, ; 258: lib_System.Reflection.Emit.dll.so => 95
	i64 u0x43375950ec7c1b6a, ; 259: netstandard.dll => 171
	i64 u0x434c4e1d9284cdae, ; 260: Mono.Android.dll => 175
	i64 u0x43505013578652a0, ; 261: lib_Xamarin.AndroidX.Activity.Ktx.dll.so => 207
	i64 u0x437d06c381ed575a, ; 262: lib_Microsoft.VisualBasic.dll.so => 3
	i64 u0x43950f84de7cc79a, ; 263: pl/Microsoft.Maui.Controls.resources.dll => 314
	i64 u0x43c077442b230f64, ; 264: Xamarin.AndroidX.Tracing.Tracing.Android => 271
	i64 u0x43e8ca5bc927ff37, ; 265: lib_Xamarin.AndroidX.Emoji2.ViewsHelper.dll.so => 233
	i64 u0x448bd33429269b19, ; 266: Microsoft.CSharp => 1
	i64 u0x4499fa3c8e494654, ; 267: lib_System.Runtime.Serialization.Primitives.dll.so => 116
	i64 u0x4515080865a951a5, ; 268: Xamarin.Kotlin.StdLib.dll => 288
	i64 u0x4545802489b736b9, ; 269: Xamarin.AndroidX.Fragment.Ktx => 236
	i64 u0x454b4d1e66bb783c, ; 270: Xamarin.AndroidX.Lifecycle.Process => 243
	i64 u0x45c40276a42e283e, ; 271: System.Diagnostics.TraceSource => 33
	i64 u0x45d443f2a29adc37, ; 272: System.AppContext.dll => 6
	i64 u0x46a4213bc97fe5ae, ; 273: lib-ru-Microsoft.Maui.Controls.resources.dll.so => 318
	i64 u0x47358bd471172e1d, ; 274: lib_System.Xml.Linq.dll.so => 159
	i64 u0x47daf4e1afbada10, ; 275: pt/Microsoft.Maui.Controls.resources => 316
	i64 u0x480c0a47dd42dd81, ; 276: lib_System.IO.MemoryMappedFiles.dll.so => 53
	i64 u0x49e952f19a4e2022, ; 277: System.ObjectModel => 87
	i64 u0x49f9e6948a8131e4, ; 278: lib_Xamarin.AndroidX.VersionedParcelable.dll.so => 275
	i64 u0x4a5667b2462a664b, ; 279: lib_Xamarin.AndroidX.Navigation.UI.dll.so => 259
	i64 u0x4a7a18981dbd56bc, ; 280: System.IO.Compression.FileSystem.dll => 44
	i64 u0x4aa5c60350917c06, ; 281: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll.so => 242
	i64 u0x4b07a0ed0ab33ff4, ; 282: System.Runtime.Extensions.dll => 106
	i64 u0x4b576d47ac054f3c, ; 283: System.IO.FileSystem.AccessControl => 47
	i64 u0x4b7b6532ded934b7, ; 284: System.Text.Json => 140
	i64 u0x4c2029a97af23a8d, ; 285: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android => 252
	i64 u0x4c7755cf07ad2d5f, ; 286: System.Net.Http.Json.dll => 65
	i64 u0x4cc5f15266470798, ; 287: lib_Xamarin.AndroidX.Loader.dll.so => 253
	i64 u0x4cf6f67dc77aacd2, ; 288: System.Net.NetworkInformation.dll => 70
	i64 u0x4d3183dd245425d4, ; 289: System.Net.WebSockets.Client.dll => 82
	i64 u0x4d479f968a05e504, ; 290: System.Linq.Expressions.dll => 60
	i64 u0x4d55a010ffc4faff, ; 291: System.Private.Xml => 91
	i64 u0x4d5cbe77561c5b2e, ; 292: System.Web.dll => 157
	i64 u0x4d77512dbd86ee4c, ; 293: lib_Xamarin.AndroidX.Arch.Core.Common.dll.so => 213
	i64 u0x4d7793536e79c309, ; 294: System.ServiceProcess => 135
	i64 u0x4d95fccc1f67c7ca, ; 295: System.Runtime.Loader.dll => 112
	i64 u0x4db014bf0ff1c9c1, ; 296: System.Linq.AsyncEnumerable => 59
	i64 u0x4dcf44c3c9b076a2, ; 297: it/Microsoft.Maui.Controls.resources.dll => 308
	i64 u0x4dd9247f1d2c3235, ; 298: Xamarin.AndroidX.Loader.dll => 253
	i64 u0x4e2aeee78e2c4a87, ; 299: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller => 260
	i64 u0x4e32f00cb0937401, ; 300: Mono.Android.Runtime => 174
	i64 u0x4e5eea4668ac2b18, ; 301: System.Text.Encoding.CodePages => 136
	i64 u0x4ebd0c4b82c5eefc, ; 302: lib_System.Threading.Channels.dll.so => 143
	i64 u0x4ee8eaa9c9c1151a, ; 303: System.Globalization.Calendars => 40
	i64 u0x4f21ee6ef9eb527e, ; 304: ca/Microsoft.Maui.Controls.resources => 295
	i64 u0x4fd5f3ee53d0a4f0, ; 305: SQLitePCLRaw.lib.e_sqlite3.android => 200
	i64 u0x5037f0be3c28c7a3, ; 306: lib_Microsoft.Maui.Controls.dll.so => 192
	i64 u0x50c3a29b21050d45, ; 307: System.Linq.Parallel.dll => 61
	i64 u0x5112ed116d87baf8, ; 308: CommunityToolkit.Mvvm => 178
	i64 u0x5131bbe80989093f, ; 309: Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll => 249
	i64 u0x516324a5050a7e3c, ; 310: System.Net.WebProxy => 81
	i64 u0x516d6f0b21a303de, ; 311: lib_System.Diagnostics.Contracts.dll.so => 25
	i64 u0x51bb8a2afe774e32, ; 312: System.Drawing => 36
	i64 u0x5247c5c32a4140f0, ; 313: System.Resources.Reader => 101
	i64 u0x526bb15e3c386364, ; 314: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll => 246
	i64 u0x526ce79eb8e90527, ; 315: lib_System.Net.Primitives.dll.so => 72
	i64 u0x52829f00b4467c38, ; 316: lib_System.Data.Common.dll.so => 22
	i64 u0x529ffe06f39ab8db, ; 317: Xamarin.AndroidX.Core => 224
	i64 u0x52ff996554dbf352, ; 318: Microsoft.Maui.Graphics => 196
	i64 u0x535f7e40e8fef8af, ; 319: lib-sk-Microsoft.Maui.Controls.resources.dll.so => 319
	i64 u0x53978aac584c666e, ; 320: lib_System.Security.Cryptography.Cng.dll.so => 123
	i64 u0x53a96d5c86c9e194, ; 321: System.Net.NetworkInformation => 70
	i64 u0x53be1038a61e8d44, ; 322: System.Runtime.InteropServices.RuntimeInformation.dll => 109
	i64 u0x53c3014b9437e684, ; 323: lib-zh-HK-Microsoft.Maui.Controls.resources.dll.so => 325
	i64 u0x5435e6f049e9bc37, ; 324: System.Security.Claims.dll => 121
	i64 u0x54795225dd1587af, ; 325: lib_System.Runtime.dll.so => 119
	i64 u0x547a34f14e5f6210, ; 326: Xamarin.AndroidX.Lifecycle.Common.dll => 238
	i64 u0x54b851bc9b470503, ; 327: Xamarin.AndroidX.Navigation.Common.Android => 255
	i64 u0x556e8b63b660ab8b, ; 328: Xamarin.AndroidX.Lifecycle.Common.Jvm.dll => 239
	i64 u0x5588627c9a108ec9, ; 329: System.Collections.Specialized => 11
	i64 u0x55a898e4f42e3fae, ; 330: Microsoft.VisualBasic.Core.dll => 2
	i64 u0x55fa0c610fe93bb1, ; 331: lib_System.Security.Cryptography.OpenSsl.dll.so => 126
	i64 u0x56442b99bc64bb47, ; 332: System.Runtime.Serialization.Xml.dll => 117
	i64 u0x56a8b26e1aeae27b, ; 333: System.Threading.Tasks.Dataflow => 145
	i64 u0x56f932d61e93c07f, ; 334: System.Globalization.Extensions => 41
	i64 u0x571c5cfbec5ae8e2, ; 335: System.Private.Uri => 89
	i64 u0x576499c9f52fea31, ; 336: Xamarin.AndroidX.Annotation => 208
	i64 u0x578cd35c91d7b347, ; 337: lib_SQLitePCLRaw.core.dll.so => 199
	i64 u0x579a06fed6eec900, ; 338: System.Private.CoreLib.dll => 177
	i64 u0x57adda3c951abb33, ; 339: Microsoft.Extensions.Hosting.Abstractions.dll => 186
	i64 u0x57c542c14049b66d, ; 340: System.Diagnostics.DiagnosticSource => 27
	i64 u0x581a8bd5cfda563e, ; 341: System.Threading.Timer => 151
	i64 u0x58601b2dda4a27b9, ; 342: lib-ja-Microsoft.Maui.Controls.resources.dll.so => 309
	i64 u0x58688d9af496b168, ; 343: Microsoft.Extensions.DependencyInjection.dll => 182
	i64 u0x588c167a79db6bfb, ; 344: lib_Xamarin.Google.ErrorProne.Annotations.dll.so => 284
	i64 u0x5906028ae5151104, ; 345: Xamarin.AndroidX.Activity.Ktx => 207
	i64 u0x595a356d23e8da9a, ; 346: lib_Microsoft.CSharp.dll.so => 1
	i64 u0x59f9e60b9475085f, ; 347: lib_Xamarin.AndroidX.Annotation.Experimental.dll.so => 209
	i64 u0x5a745f5101a75527, ; 348: lib_System.IO.Compression.FileSystem.dll.so => 44
	i64 u0x5a89a886ae30258d, ; 349: lib_Xamarin.AndroidX.CoordinatorLayout.dll.so => 223
	i64 u0x5a8f6699f4a1caa9, ; 350: lib_System.Threading.dll.so => 152
	i64 u0x5ae9cd33b15841bf, ; 351: System.ComponentModel => 18
	i64 u0x5b54391bdc6fcfe6, ; 352: System.Private.DataContractSerialization => 88
	i64 u0x5b5f0e240a06a2a2, ; 353: da/Microsoft.Maui.Controls.resources.dll => 297
	i64 u0x5b8109e8e14c5e3e, ; 354: System.Globalization.Extensions.dll => 41
	i64 u0x5bddd04d72a9e350, ; 355: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx => 242
	i64 u0x5bdf16b09da116ab, ; 356: Xamarin.AndroidX.Collection => 217
	i64 u0x5c019d5266093159, ; 357: lib_Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android.dll.so => 247
	i64 u0x5c30a4a35f9cc8c4, ; 358: lib_System.Reflection.Extensions.dll.so => 96
	i64 u0x5c393624b8176517, ; 359: lib_Microsoft.Extensions.Logging.dll.so => 187
	i64 u0x5c53c29f5073b0c9, ; 360: System.Diagnostics.FileVersionInfo => 28
	i64 u0x5c87463c575c7616, ; 361: lib_System.Globalization.Extensions.dll.so => 41
	i64 u0x5d0a4a29b02d9d3c, ; 362: System.Net.WebHeaderCollection.dll => 80
	i64 u0x5d40c9b15181641f, ; 363: lib_Xamarin.AndroidX.Emoji2.dll.so => 232
	i64 u0x5d6ca10d35e9485b, ; 364: lib_Xamarin.AndroidX.Concurrent.Futures.dll.so => 220
	i64 u0x5d7ec76c1c703055, ; 365: System.Threading.Tasks.Parallel => 147
	i64 u0x5db0cbbd1028510e, ; 366: lib_System.Runtime.InteropServices.dll.so => 110
	i64 u0x5db30905d3e5013b, ; 367: Xamarin.AndroidX.Collection.Jvm.dll => 218
	i64 u0x5e467bc8f09ad026, ; 368: System.Collections.Specialized.dll => 11
	i64 u0x5e5173b3208d97e7, ; 369: System.Runtime.Handles.dll => 107
	i64 u0x5ea92fdb19ec8c4c, ; 370: System.Text.Encodings.Web.dll => 139
	i64 u0x5eb8046dd40e9ac3, ; 371: System.ComponentModel.Primitives => 16
	i64 u0x5ec272d219c9aba4, ; 372: System.Security.Cryptography.Csp.dll => 124
	i64 u0x5eee1376d94c7f5e, ; 373: System.Net.HttpListener.dll => 67
	i64 u0x5f36ccf5c6a57e24, ; 374: System.Xml.ReaderWriter.dll => 160
	i64 u0x5f4294b9b63cb842, ; 375: System.Data.Common => 22
	i64 u0x5f7399e166075632, ; 376: lib_SQLitePCLRaw.lib.e_sqlite3.android.dll.so => 200
	i64 u0x5f9a2d823f664957, ; 377: lib-el-Microsoft.Maui.Controls.resources.dll.so => 299
	i64 u0x5fa6da9c3cd8142a, ; 378: lib_Xamarin.KotlinX.Serialization.Core.dll.so => 292
	i64 u0x5fac98e0b37a5b9d, ; 379: System.Runtime.CompilerServices.Unsafe.dll => 104
	i64 u0x609f4b7b63d802d4, ; 380: lib_Microsoft.Extensions.DependencyInjection.dll.so => 182
	i64 u0x60cd4e33d7e60134, ; 381: Xamarin.KotlinX.Coroutines.Core.Jvm => 291
	i64 u0x60f62d786afcf130, ; 382: System.Memory => 64
	i64 u0x61bb78c89f867353, ; 383: System.IO => 58
	i64 u0x61be8d1299194243, ; 384: Microsoft.Maui.Controls.Xaml => 193
	i64 u0x61d2cba29557038f, ; 385: de/Microsoft.Maui.Controls.resources => 298
	i64 u0x61d88f399afb2f45, ; 386: lib_System.Runtime.Loader.dll.so => 112
	i64 u0x622eef6f9e59068d, ; 387: System.Private.CoreLib => 177
	i64 u0x639fb99a7bef11de, ; 388: Xamarin.AndroidX.Navigation.Runtime.Android.dll => 258
	i64 u0x63d5e3aa4ef9b931, ; 389: Xamarin.KotlinX.Coroutines.Android.dll => 289
	i64 u0x63f1f6883c1e23c2, ; 390: lib_System.Collections.Immutable.dll.so => 9
	i64 u0x6400f68068c1e9f1, ; 391: Xamarin.Google.Android.Material.dll => 281
	i64 u0x640e3b14dbd325c2, ; 392: System.Security.Cryptography.Algorithms.dll => 122
	i64 u0x64587004560099b9, ; 393: System.Reflection => 100
	i64 u0x64b1529a438a3c45, ; 394: lib_System.Runtime.Handles.dll.so => 107
	i64 u0x64b61dd9da8a4d57, ; 395: System.Net.ServerSentEvents.dll => 76
	i64 u0x6565fba2cd8f235b, ; 396: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx => 250
	i64 u0x65ecac39144dd3cc, ; 397: Microsoft.Maui.Controls.dll => 192
	i64 u0x65ece51227bfa724, ; 398: lib_System.Runtime.Numerics.dll.so => 113
	i64 u0x661722438787b57f, ; 399: Xamarin.AndroidX.Annotation.Jvm.dll => 210
	i64 u0x6679b2337ee6b22a, ; 400: lib_System.IO.FileSystem.Primitives.dll.so => 49
	i64 u0x667c66a03dd97d40, ; 401: System.Linq.AsyncEnumerable.dll => 59
	i64 u0x6692e924eade1b29, ; 402: lib_System.Console.dll.so => 20
	i64 u0x66a4e5c6a3fb0bae, ; 403: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll.so => 249
	i64 u0x66d13304ce1a3efa, ; 404: Xamarin.AndroidX.CursorAdapter => 227
	i64 u0x674303f65d8fad6f, ; 405: lib_System.Net.Quic.dll.so => 73
	i64 u0x6756ca4cad62e9d6, ; 406: lib_Xamarin.AndroidX.ConstraintLayout.Core.dll.so => 222
	i64 u0x67c0802770244408, ; 407: System.Windows.dll => 158
	i64 u0x68100b69286e27cd, ; 408: lib_System.Formats.Tar.dll.so => 39
	i64 u0x68558ec653afa616, ; 409: lib-da-Microsoft.Maui.Controls.resources.dll.so => 297
	i64 u0x6872ec7a2e36b1ac, ; 410: System.Drawing.Primitives.dll => 35
	i64 u0x68fbbbe2eb455198, ; 411: System.Formats.Asn1 => 38
	i64 u0x69063fc0ba8e6bdd, ; 412: he/Microsoft.Maui.Controls.resources.dll => 303
	i64 u0x699dffb2427a2d71, ; 413: SQLitePCLRaw.lib.e_sqlite3.android.dll => 200
	i64 u0x6a4d7577b2317255, ; 414: System.Runtime.InteropServices.dll => 110
	i64 u0x6ace3b74b15ee4a4, ; 415: nb/Microsoft.Maui.Controls.resources => 312
	i64 u0x6afcedb171067e2b, ; 416: System.Core.dll => 21
	i64 u0x6bef98e124147c24, ; 417: Xamarin.Jetbrains.Annotations => 286
	i64 u0x6ce874bff138ce2b, ; 418: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 248
	i64 u0x6d12bfaa99c72b1f, ; 419: lib_Microsoft.Maui.Graphics.dll.so => 196
	i64 u0x6d70755158ca866e, ; 420: lib_System.ComponentModel.EventBasedAsync.dll.so => 15
	i64 u0x6d79993361e10ef2, ; 421: Microsoft.Extensions.Primitives => 191
	i64 u0x6d7eeca99577fc8b, ; 422: lib_System.Net.WebProxy.dll.so => 81
	i64 u0x6d8515b19946b6a2, ; 423: System.Net.WebProxy.dll => 81
	i64 u0x6d86d56b84c8eb71, ; 424: lib_Xamarin.AndroidX.CursorAdapter.dll.so => 227
	i64 u0x6d9bea6b3e895cf7, ; 425: Microsoft.Extensions.Primitives.dll => 191
	i64 u0x6e25a02c3833319a, ; 426: lib_Xamarin.AndroidX.Navigation.Fragment.dll.so => 256
	i64 u0x6e79c6bd8627412a, ; 427: Xamarin.AndroidX.SavedState.SavedState.Ktx => 265
	i64 u0x6e838d9a2a6f6c9e, ; 428: lib_System.ValueTuple.dll.so => 155
	i64 u0x6e9965ce1095e60a, ; 429: lib_System.Core.dll.so => 21
	i64 u0x6fd2265da78b93a4, ; 430: lib_Microsoft.Maui.dll.so => 194
	i64 u0x6fdfc7de82c33008, ; 431: cs/Microsoft.Maui.Controls.resources => 296
	i64 u0x6ffc4967cc47ba57, ; 432: System.IO.FileSystem.Watcher.dll => 50
	i64 u0x701cd46a1c25a5fe, ; 433: System.IO.FileSystem.dll => 51
	i64 u0x70e99f48c05cb921, ; 434: tr/Microsoft.Maui.Controls.resources.dll => 322
	i64 u0x70fd3deda22442d2, ; 435: lib-nb-Microsoft.Maui.Controls.resources.dll.so => 312
	i64 u0x71485e7ffdb4b958, ; 436: System.Reflection.Extensions => 96
	i64 u0x7162a2fce67a945f, ; 437: lib_Xamarin.Android.Glide.Annotations.dll.so => 203
	i64 u0x717530326f808838, ; 438: lib_Microsoft.Extensions.Diagnostics.Abstractions.dll.so => 184
	i64 u0x71a495ea3761dde8, ; 439: lib-it-Microsoft.Maui.Controls.resources.dll.so => 308
	i64 u0x71ad672adbe48f35, ; 440: System.ComponentModel.Primitives.dll => 16
	i64 u0x720f102581a4a5c8, ; 441: Xamarin.AndroidX.Core.ViewTree => 226
	i64 u0x725f5a9e82a45c81, ; 442: System.Security.Cryptography.Encoding => 125
	i64 u0x72b1fb4109e08d7b, ; 443: lib-hr-Microsoft.Maui.Controls.resources.dll.so => 305
	i64 u0x72e0300099accce1, ; 444: System.Xml.XPath.XDocument => 163
	i64 u0x730bfb248998f67a, ; 445: System.IO.Compression.ZipFile => 45
	i64 u0x732b2d67b9e5c47b, ; 446: Xamarin.Google.ErrorProne.Annotations.dll => 284
	i64 u0x734b76fdc0dc05bb, ; 447: lib_GoogleGson.dll.so => 179
	i64 u0x73a6be34e822f9d1, ; 448: lib_System.Runtime.Serialization.dll.so => 118
	i64 u0x73e4ce94e2eb6ffc, ; 449: lib_System.Memory.dll.so => 64
	i64 u0x743a1eccf080489a, ; 450: WindowsBase.dll => 169
	i64 u0x755a91767330b3d4, ; 451: lib_Microsoft.Extensions.Configuration.dll.so => 180
	i64 u0x75c326eb821b85c4, ; 452: lib_System.ComponentModel.DataAnnotations.dll.so => 14
	i64 u0x76012e7334db86e5, ; 453: lib_Xamarin.AndroidX.SavedState.dll.so => 263
	i64 u0x76ca07b878f44da0, ; 454: System.Runtime.Numerics.dll => 113
	i64 u0x7736c8a96e51a061, ; 455: lib_Xamarin.AndroidX.Annotation.Jvm.dll.so => 210
	i64 u0x778a805e625329ef, ; 456: System.Linq.Parallel => 61
	i64 u0x77d9074d8f33a303, ; 457: lib_System.Net.ServerSentEvents.dll.so => 76
	i64 u0x77f8a4acc2fdc449, ; 458: System.Security.Cryptography.Cng.dll => 123
	i64 u0x780bc73597a503a9, ; 459: lib-ms-Microsoft.Maui.Controls.resources.dll.so => 311
	i64 u0x782c5d8eb99ff201, ; 460: lib_Microsoft.VisualBasic.Core.dll.so => 2
	i64 u0x783606d1e53e7a1a, ; 461: th/Microsoft.Maui.Controls.resources.dll => 321
	i64 u0x78a45e51311409b6, ; 462: Xamarin.AndroidX.Fragment.dll => 235
	i64 u0x78ed4ab8f9d800a1, ; 463: Xamarin.AndroidX.Lifecycle.ViewModel => 248
	i64 u0x7a5207a7c82d30b4, ; 464: lib_Xamarin.JSpecify.dll.so => 287
	i64 u0x7a7e7eddf79c5d26, ; 465: lib_Xamarin.AndroidX.Lifecycle.ViewModel.dll.so => 248
	i64 u0x7a9a57d43b0845fa, ; 466: System.AppContext => 6
	i64 u0x7ad0f4f1e5d08183, ; 467: Xamarin.AndroidX.Collection.dll => 217
	i64 u0x7adb8da2ac89b647, ; 468: fi/Microsoft.Maui.Controls.resources.dll => 301
	i64 u0x7b13d9eaa944ade8, ; 469: Xamarin.AndroidX.DynamicAnimation.dll => 231
	i64 u0x7bef86a4335c4870, ; 470: System.ComponentModel.TypeConverter => 17
	i64 u0x7c0820144cd34d6a, ; 471: sk/Microsoft.Maui.Controls.resources.dll => 319
	i64 u0x7c2a0bd1e0f988fc, ; 472: lib-de-Microsoft.Maui.Controls.resources.dll.so => 298
	i64 u0x7c41d387501568ba, ; 473: System.Net.WebClient.dll => 79
	i64 u0x7c482cd79bd24b13, ; 474: lib_Xamarin.AndroidX.ConstraintLayout.dll.so => 221
	i64 u0x7c60acf6404e96b6, ; 475: Xamarin.AndroidX.Navigation.Common.Android.dll => 255
	i64 u0x7cd2ec8eaf5241cd, ; 476: System.Security.dll => 133
	i64 u0x7cf9ae50dd350622, ; 477: Xamarin.Jetbrains.Annotations.dll => 286
	i64 u0x7d649b75d580bb42, ; 478: ms/Microsoft.Maui.Controls.resources.dll => 311
	i64 u0x7d8ee2bdc8e3aad1, ; 479: System.Numerics.Vectors => 85
	i64 u0x7df5df8db8eaa6ac, ; 480: Microsoft.Extensions.Logging.Debug => 189
	i64 u0x7dfc3d6d9d8d7b70, ; 481: System.Collections => 12
	i64 u0x7e2e564fa2f76c65, ; 482: lib_System.Diagnostics.Tracing.dll.so => 34
	i64 u0x7e302e110e1e1346, ; 483: lib_System.Security.Claims.dll.so => 121
	i64 u0x7e4465b3f78ad8d0, ; 484: Xamarin.KotlinX.Serialization.Core.dll => 292
	i64 u0x7e571cad5915e6c3, ; 485: lib_Xamarin.AndroidX.Lifecycle.Process.dll.so => 243
	i64 u0x7e6ac99e4e8df72f, ; 486: System.IO.Hashing => 176
	i64 u0x7e6b1ca712437d7d, ; 487: Xamarin.AndroidX.Emoji2.ViewsHelper => 233
	i64 u0x7e946809d6008ef2, ; 488: lib_System.ObjectModel.dll.so => 87
	i64 u0x7ea0272c1b4a9635, ; 489: lib_Xamarin.Android.Glide.dll.so => 202
	i64 u0x7ecc13347c8fd849, ; 490: lib_System.ComponentModel.dll.so => 18
	i64 u0x7f00ddd9b9ca5a13, ; 491: Xamarin.AndroidX.ViewPager.dll => 276
	i64 u0x7f9351cd44b1273f, ; 492: Microsoft.Extensions.Configuration.Abstractions => 181
	i64 u0x7fbd557c99b3ce6f, ; 493: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.dll.so => 241
	i64 u0x8076a9a44a2ca331, ; 494: System.Net.Quic => 73
	i64 u0x80da183a87731838, ; 495: System.Reflection.Metadata => 97
	i64 u0x80fa55b6d1b0be99, ; 496: SQLitePCLRaw.provider.e_sqlite3 => 201
	i64 u0x812c069d5cdecc17, ; 497: System.dll => 168
	i64 u0x81381be520a60adb, ; 498: Xamarin.AndroidX.Interpolator.dll => 237
	i64 u0x81657cec2b31e8aa, ; 499: System.Net => 84
	i64 u0x81ab745f6c0f5ce6, ; 500: zh-Hant/Microsoft.Maui.Controls.resources => 327
	i64 u0x8277f2be6b5ce05f, ; 501: Xamarin.AndroidX.AppCompat => 211
	i64 u0x828f06563b30bc50, ; 502: lib_Xamarin.AndroidX.CardView.dll.so => 216
	i64 u0x82b399cb01b531c4, ; 503: lib_System.Web.dll.so => 157
	i64 u0x82df8f5532a10c59, ; 504: lib_System.Drawing.dll.so => 36
	i64 u0x82f0b6e911d13535, ; 505: lib_System.Transactions.dll.so => 154
	i64 u0x82f6403342e12049, ; 506: uk/Microsoft.Maui.Controls.resources => 323
	i64 u0x83144699b312ad81, ; 507: SQLite-net.dll => 197
	i64 u0x83c14ba66c8e2b8c, ; 508: zh-Hans/Microsoft.Maui.Controls.resources => 326
	i64 u0x846ce984efea52c7, ; 509: System.Threading.Tasks.Parallel.dll => 147
	i64 u0x84ae73148a4557d2, ; 510: lib_System.IO.Pipes.dll.so => 56
	i64 u0x84b01102c12a9232, ; 511: System.Runtime.Serialization.Json.dll => 115
	i64 u0x850c5ba0b57ce8e7, ; 512: lib_Xamarin.AndroidX.Collection.dll.so => 217
	i64 u0x851d02edd334b044, ; 513: Xamarin.AndroidX.VectorDrawable => 273
	i64 u0x85c919db62150978, ; 514: Xamarin.AndroidX.Transition.dll => 272
	i64 u0x8662aaeb94fef37f, ; 515: lib_System.Dynamic.Runtime.dll.so => 37
	i64 u0x86a909228dc7657b, ; 516: lib-zh-Hant-Microsoft.Maui.Controls.resources.dll.so => 327
	i64 u0x86b3e00c36b84509, ; 517: Microsoft.Extensions.Configuration.dll => 180
	i64 u0x86b62cb077ec4fd7, ; 518: System.Runtime.Serialization.Xml => 117
	i64 u0x8706ffb12bf3f53d, ; 519: Xamarin.AndroidX.Annotation.Experimental => 209
	i64 u0x872a5b14c18d328c, ; 520: System.ComponentModel.DataAnnotations => 14
	i64 u0x872fb9615bc2dff0, ; 521: Xamarin.Android.Glide.Annotations.dll => 203
	i64 u0x87c69b87d9283884, ; 522: lib_System.Threading.Thread.dll.so => 149
	i64 u0x87f6569b25707834, ; 523: System.IO.Compression.Brotli.dll => 43
	i64 u0x8842b3a5d2d3fb36, ; 524: Microsoft.Maui.Essentials => 195
	i64 u0x88926583efe7ee86, ; 525: Xamarin.AndroidX.Activity.Ktx.dll => 207
	i64 u0x88ba6bc4f7762b03, ; 526: lib_System.Reflection.dll.so => 100
	i64 u0x88bda98e0cffb7a9, ; 527: lib_Xamarin.KotlinX.Coroutines.Core.Jvm.dll.so => 291
	i64 u0x8930322c7bd8f768, ; 528: netstandard => 171
	i64 u0x897a606c9e39c75f, ; 529: lib_System.ComponentModel.Primitives.dll.so => 16
	i64 u0x898a5c6bc9e47ec1, ; 530: lib_Xamarin.AndroidX.SavedState.SavedState.Android.dll.so => 264
	i64 u0x89911a22005b92b7, ; 531: System.IO.FileSystem.DriveInfo.dll => 48
	i64 u0x89c5188089ec2cd5, ; 532: lib_System.Runtime.InteropServices.RuntimeInformation.dll.so => 109
	i64 u0x8a19e3dc71b34b2c, ; 533: System.Reflection.TypeExtensions.dll => 99
	i64 u0x8ad229ea26432ee2, ; 534: Xamarin.AndroidX.Loader => 253
	i64 u0x8b4ff5d0fdd5faa1, ; 535: lib_System.Diagnostics.DiagnosticSource.dll.so => 27
	i64 u0x8b541d476eb3774c, ; 536: System.Security.Principal.Windows => 130
	i64 u0x8b8d01333a96d0b5, ; 537: System.Diagnostics.Process.dll => 29
	i64 u0x8b9ceca7acae3451, ; 538: lib-he-Microsoft.Maui.Controls.resources.dll.so => 303
	i64 u0x8c575135aa1ccef4, ; 539: Microsoft.Extensions.FileProviders.Abstractions => 185
	i64 u0x8cb8f612b633affb, ; 540: Xamarin.AndroidX.SavedState.SavedState.Ktx.dll => 265
	i64 u0x8cdfdb4ce85fb925, ; 541: lib_System.Security.Principal.Windows.dll.so => 130
	i64 u0x8cdfe7b8f4caa426, ; 542: System.IO.Compression.FileSystem => 44
	i64 u0x8d0f420977c2c1c7, ; 543: Xamarin.AndroidX.CursorAdapter.dll => 227
	i64 u0x8d52f7ea2796c531, ; 544: Xamarin.AndroidX.Emoji2.dll => 232
	i64 u0x8d7b8ab4b3310ead, ; 545: System.Threading => 152
	i64 u0x8da188285aadfe8e, ; 546: System.Collections.Concurrent => 8
	i64 u0x8e8f269ad1e1ff94, ; 547: lib_Xamarin.AndroidX.Tracing.Tracing.Android.dll.so => 271
	i64 u0x8ed807bfe9858dfc, ; 548: Xamarin.AndroidX.Navigation.Common => 254
	i64 u0x8ee08b8194a30f48, ; 549: lib-hi-Microsoft.Maui.Controls.resources.dll.so => 304
	i64 u0x8ef7601039857a44, ; 550: lib-ro-Microsoft.Maui.Controls.resources.dll.so => 317
	i64 u0x8ef9414937d93a0a, ; 551: SQLitePCLRaw.core.dll => 199
	i64 u0x8f32c6f611f6ffab, ; 552: pt/Microsoft.Maui.Controls.resources.dll => 316
	i64 u0x8f44b45eb046bbd1, ; 553: System.ServiceModel.Web.dll => 134
	i64 u0x8f8829d21c8985a4, ; 554: lib-pt-BR-Microsoft.Maui.Controls.resources.dll.so => 315
	i64 u0x8fbf5b0114c6dcef, ; 555: System.Globalization.dll => 42
	i64 u0x8fcc8c2a81f3d9e7, ; 556: Xamarin.KotlinX.Serialization.Core => 292
	i64 u0x8fd27d934d7b3a55, ; 557: SQLitePCLRaw.core => 199
	i64 u0x90263f8448b8f572, ; 558: lib_System.Diagnostics.TraceSource.dll.so => 33
	i64 u0x903101b46fb73a04, ; 559: _Microsoft.Android.Resource.Designer => 328
	i64 u0x90393bd4865292f3, ; 560: lib_System.IO.Compression.dll.so => 46
	i64 u0x905e2b8e7ae91ae6, ; 561: System.Threading.Tasks.Extensions.dll => 146
	i64 u0x90634f86c5ebe2b5, ; 562: Xamarin.AndroidX.Lifecycle.ViewModel.Android => 249
	i64 u0x907b636704ad79ef, ; 563: lib_Microsoft.Maui.Controls.Xaml.dll.so => 193
	i64 u0x90e9efbfd68593e0, ; 564: lib_Xamarin.AndroidX.Lifecycle.LiveData.dll.so => 240
	i64 u0x91418dc638b29e68, ; 565: lib_Xamarin.AndroidX.CustomView.dll.so => 228
	i64 u0x9157bd523cd7ed36, ; 566: lib_System.Text.Json.dll.so => 140
	i64 u0x91a74f07b30d37e2, ; 567: System.Linq.dll => 63
	i64 u0x91cb86ea3b17111d, ; 568: System.ServiceModel.Web => 134
	i64 u0x91fa41a87223399f, ; 569: ca/Microsoft.Maui.Controls.resources.dll => 295
	i64 u0x92054e486c0c7ea7, ; 570: System.IO.FileSystem.DriveInfo => 48
	i64 u0x928614058c40c4cd, ; 571: lib_System.Xml.XPath.XDocument.dll.so => 163
	i64 u0x92b138fffca2b01e, ; 572: lib_Xamarin.AndroidX.Arch.Core.Runtime.dll.so => 214
	i64 u0x92dfc2bfc6c6a888, ; 573: Xamarin.AndroidX.Lifecycle.LiveData => 240
	i64 u0x933da2c779423d68, ; 574: Xamarin.Android.Glide.Annotations => 203
	i64 u0x9388aad9b7ae40ce, ; 575: lib_Xamarin.AndroidX.Lifecycle.Common.dll.so => 238
	i64 u0x93cfa73ab28d6e35, ; 576: ms/Microsoft.Maui.Controls.resources => 311
	i64 u0x941c00d21e5c0679, ; 577: lib_Xamarin.AndroidX.Transition.dll.so => 272
	i64 u0x944077d8ca3c6580, ; 578: System.IO.Compression.dll => 46
	i64 u0x948cffedc8ed7960, ; 579: System.Xml => 167
	i64 u0x94bbeab0d4764588, ; 580: System.IO.Hashing.dll => 176
	i64 u0x94c8990839c4bdb1, ; 581: lib_Xamarin.AndroidX.Interpolator.dll.so => 237
	i64 u0x967fc325e09bfa8c, ; 582: es/Microsoft.Maui.Controls.resources => 300
	i64 u0x9686161486d34b81, ; 583: lib_Xamarin.AndroidX.ExifInterface.dll.so => 234
	i64 u0x9732d8dbddea3d9a, ; 584: id/Microsoft.Maui.Controls.resources => 307
	i64 u0x978be80e5210d31b, ; 585: Microsoft.Maui.Graphics.dll => 196
	i64 u0x97b8c771ea3e4220, ; 586: System.ComponentModel.dll => 18
	i64 u0x97e144c9d3c6976e, ; 587: System.Collections.Concurrent.dll => 8
	i64 u0x984184e3c70d4419, ; 588: GoogleGson => 179
	i64 u0x9843944103683dd3, ; 589: Xamarin.AndroidX.Core.Core.Ktx => 225
	i64 u0x98b05cc81e6f333c, ; 590: Xamarin.AndroidX.SavedState.SavedState.Android.dll => 264
	i64 u0x98d720cc4597562c, ; 591: System.Security.Cryptography.OpenSsl => 126
	i64 u0x991d510397f92d9d, ; 592: System.Linq.Expressions => 60
	i64 u0x996ceeb8a3da3d67, ; 593: System.Threading.Overlapped.dll => 144
	i64 u0x999cb19e1a04ffd3, ; 594: CommunityToolkit.Mvvm.dll => 178
	i64 u0x99a00ca5270c6878, ; 595: Xamarin.AndroidX.Navigation.Runtime => 257
	i64 u0x99cdc6d1f2d3a72f, ; 596: ko/Microsoft.Maui.Controls.resources.dll => 310
	i64 u0x9a01b1da98b6ee10, ; 597: Xamarin.AndroidX.Lifecycle.Runtime.dll => 244
	i64 u0x9a5ccc274fd6e6ee, ; 598: Jsr305Binding.dll => 282
	i64 u0x9ae6940b11c02876, ; 599: lib_Xamarin.AndroidX.Window.dll.so => 278
	i64 u0x9b211a749105beac, ; 600: System.Transactions.Local => 153
	i64 u0x9b8734714671022d, ; 601: System.Threading.Tasks.Dataflow.dll => 145
	i64 u0x9bc6aea27fbf034f, ; 602: lib_Xamarin.KotlinX.Coroutines.Core.dll.so => 290
	i64 u0x9c244ac7cda32d26, ; 603: System.Security.Cryptography.X509Certificates.dll => 128
	i64 u0x9c465f280cf43733, ; 604: lib_Xamarin.KotlinX.Coroutines.Android.dll.so => 289
	i64 u0x9c8f6872beab6408, ; 605: System.Xml.XPath.XDocument.dll => 163
	i64 u0x9ce01cf91101ae23, ; 606: System.Xml.XmlDocument => 165
	i64 u0x9d128180c81d7ce6, ; 607: Xamarin.AndroidX.CustomView.PoolingContainer => 229
	i64 u0x9d5dbcf5a48583fe, ; 608: lib_Xamarin.AndroidX.Activity.dll.so => 206
	i64 u0x9d74dee1a7725f34, ; 609: Microsoft.Extensions.Configuration.Abstractions.dll => 181
	i64 u0x9dd0e195825d65c6, ; 610: lib_Xamarin.AndroidX.Navigation.Runtime.Android.dll.so => 258
	i64 u0x9e4534b6adaf6e84, ; 611: nl/Microsoft.Maui.Controls.resources => 313
	i64 u0x9e4b95dec42769f7, ; 612: System.Diagnostics.Debug.dll => 26
	i64 u0x9eaf1efdf6f7267e, ; 613: Xamarin.AndroidX.Navigation.Common.dll => 254
	i64 u0x9ef542cf1f78c506, ; 614: Xamarin.AndroidX.Lifecycle.LiveData.Core => 241
	i64 u0xa00832eb975f56a8, ; 615: lib_System.Net.dll.so => 84
	i64 u0xa0ad78236b7b267f, ; 616: Xamarin.AndroidX.Window => 278
	i64 u0xa0d8259f4cc284ec, ; 617: lib_System.Security.Cryptography.dll.so => 129
	i64 u0xa0e17ca50c77a225, ; 618: lib_Xamarin.Google.Crypto.Tink.Android.dll.so => 283
	i64 u0xa0ff9b3e34d92f11, ; 619: lib_System.Resources.Writer.dll.so => 103
	i64 u0xa12fbfb4da97d9f3, ; 620: System.Threading.Timer.dll => 151
	i64 u0xa1440773ee9d341e, ; 621: Xamarin.Google.Android.Material => 281
	i64 u0xa18c39c44cdc3465, ; 622: Xamarin.AndroidX.Window.WindowCore => 279
	i64 u0xa1b9d7c27f47219f, ; 623: Xamarin.AndroidX.Navigation.UI.dll => 259
	i64 u0xa2572680829d2c7c, ; 624: System.IO.Pipelines.dll => 54
	i64 u0xa26597e57ee9c7f6, ; 625: System.Xml.XmlDocument.dll => 165
	i64 u0xa308401900e5bed3, ; 626: lib_mscorlib.dll.so => 170
	i64 u0xa395572e7da6c99d, ; 627: lib_System.Security.dll.so => 133
	i64 u0xa3e683f24b43af6f, ; 628: System.Dynamic.Runtime.dll => 37
	i64 u0xa4145becdee3dc4f, ; 629: Xamarin.AndroidX.VectorDrawable.Animated => 274
	i64 u0xa46aa1eaa214539b, ; 630: ko/Microsoft.Maui.Controls.resources => 310
	i64 u0xa4d20d2ff0563d26, ; 631: lib_CommunityToolkit.Mvvm.dll.so => 178
	i64 u0xa4edc8f2ceae241a, ; 632: System.Data.Common.dll => 22
	i64 u0xa5494f40f128ce6a, ; 633: System.Runtime.Serialization.Formatters.dll => 114
	i64 u0xa54b74df83dce92b, ; 634: System.Reflection.DispatchProxy => 92
	i64 u0xa5b7152421ed6d98, ; 635: lib_System.IO.FileSystem.Watcher.dll.so => 50
	i64 u0xa5c3844f17b822db, ; 636: lib_System.Linq.Parallel.dll.so => 61
	i64 u0xa5ce5c755bde8cb8, ; 637: lib_System.Security.Cryptography.Csp.dll.so => 124
	i64 u0xa5e599d1e0524750, ; 638: System.Numerics.Vectors.dll => 85
	i64 u0xa5f1ba49b85dd355, ; 639: System.Security.Cryptography.dll => 129
	i64 u0xa61975a5a37873ea, ; 640: lib_System.Xml.XmlSerializer.dll.so => 166
	i64 u0xa6593e21584384d2, ; 641: lib_Jsr305Binding.dll.so => 282
	i64 u0xa66cbee0130865f7, ; 642: lib_WindowsBase.dll.so => 169
	i64 u0xa67dbee13e1df9ca, ; 643: Xamarin.AndroidX.SavedState.dll => 263
	i64 u0xa684b098dd27b296, ; 644: lib_Xamarin.AndroidX.Security.SecurityCrypto.dll.so => 266
	i64 u0xa68a420042bb9b1f, ; 645: Xamarin.AndroidX.DrawerLayout.dll => 230
	i64 u0xa6d26156d1cacc7c, ; 646: Xamarin.Android.Glide.dll => 202
	i64 u0xa75386b5cb9595aa, ; 647: Xamarin.AndroidX.Lifecycle.Runtime.Android => 245
	i64 u0xa763fbb98df8d9fb, ; 648: lib_Microsoft.Win32.Primitives.dll.so => 4
	i64 u0xa78ce3745383236a, ; 649: Xamarin.AndroidX.Lifecycle.Common.Jvm => 239
	i64 u0xa7c31b56b4dc7b33, ; 650: hu/Microsoft.Maui.Controls.resources => 306
	i64 u0xa7eab29ed44b4e7a, ; 651: Mono.Android.Export => 173
	i64 u0xa8195217cbf017b7, ; 652: Microsoft.VisualBasic.Core => 2
	i64 u0xa859a95830f367ff, ; 653: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll.so => 250
	i64 u0xa891a7bfe327a405, ; 654: App_v3.dll => 0
	i64 u0xa8b52f21e0dbe690, ; 655: System.Runtime.Serialization.dll => 118
	i64 u0xa8ee4ed7de2efaee, ; 656: Xamarin.AndroidX.Annotation.dll => 208
	i64 u0xa95590e7c57438a4, ; 657: System.Configuration => 19
	i64 u0xaa2219c8e3449ff5, ; 658: Microsoft.Extensions.Logging.Abstractions => 188
	i64 u0xaa443ac34067eeef, ; 659: System.Private.Xml.dll => 91
	i64 u0xaa52de307ef5d1dd, ; 660: System.Net.Http => 66
	i64 u0xaa9a7b0214a5cc5c, ; 661: System.Diagnostics.StackTrace.dll => 30
	i64 u0xaaaf86367285a918, ; 662: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 183
	i64 u0xaaf84bb3f052a265, ; 663: el/Microsoft.Maui.Controls.resources => 299
	i64 u0xab9af77b5b67a0b8, ; 664: Xamarin.AndroidX.ConstraintLayout.Core => 222
	i64 u0xab9c1b2687d86b0b, ; 665: lib_System.Linq.Expressions.dll.so => 60
	i64 u0xac2af3fa195a15ce, ; 666: System.Runtime.Numerics => 113
	i64 u0xac5376a2a538dc10, ; 667: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 241
	i64 u0xac5acae88f60357e, ; 668: System.Diagnostics.Tools.dll => 32
	i64 u0xac79c7e46047ad98, ; 669: System.Security.Principal.Windows.dll => 130
	i64 u0xac98d31068e24591, ; 670: System.Xml.XDocument => 162
	i64 u0xacd46e002c3ccb97, ; 671: ro/Microsoft.Maui.Controls.resources => 317
	i64 u0xacdd9e4180d56dda, ; 672: Xamarin.AndroidX.Concurrent.Futures => 220
	i64 u0xacf42eea7ef9cd12, ; 673: System.Threading.Channels => 143
	i64 u0xad89c07347f1bad6, ; 674: nl/Microsoft.Maui.Controls.resources.dll => 313
	i64 u0xadbb53caf78a79d2, ; 675: System.Web.HttpUtility => 156
	i64 u0xadc90ab061a9e6e4, ; 676: System.ComponentModel.TypeConverter.dll => 17
	i64 u0xadca1b9030b9317e, ; 677: Xamarin.AndroidX.Collection.Ktx => 219
	i64 u0xadd8eda2edf396ad, ; 678: Xamarin.Android.Glide.GifDecoder => 205
	i64 u0xadf4cf30debbeb9a, ; 679: System.Net.ServicePoint.dll => 77
	i64 u0xadf511667bef3595, ; 680: System.Net.Security => 75
	i64 u0xae0aaa94fdcfce0f, ; 681: System.ComponentModel.EventBasedAsync.dll => 15
	i64 u0xae282bcd03739de7, ; 682: Java.Interop => 172
	i64 u0xae53579c90db1107, ; 683: System.ObjectModel.dll => 87
	i64 u0xae7ea18c61eef394, ; 684: SQLite-net => 197
	i64 u0xaf732d0b2193b8f5, ; 685: System.Security.Cryptography.OpenSsl.dll => 126
	i64 u0xafdb94dbccd9d11c, ; 686: Xamarin.AndroidX.Lifecycle.LiveData.dll => 240
	i64 u0xafe29f45095518e7, ; 687: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll.so => 251
	i64 u0xb03ae931fb25607e, ; 688: Xamarin.AndroidX.ConstraintLayout => 221
	i64 u0xb05cc42cd94c6d9d, ; 689: lib-sv-Microsoft.Maui.Controls.resources.dll.so => 320
	i64 u0xb0ac21bec8f428c5, ; 690: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android.dll => 247
	i64 u0xb0bb43dc52ea59f9, ; 691: System.Diagnostics.Tracing.dll => 34
	i64 u0xb1dd05401aa8ee63, ; 692: System.Security.AccessControl => 120
	i64 u0xb220631954820169, ; 693: System.Text.RegularExpressions => 141
	i64 u0xb2376e1dbf8b4ed7, ; 694: System.Security.Cryptography.Csp => 124
	i64 u0xb2a1959fe95c5402, ; 695: lib_System.Runtime.InteropServices.JavaScript.dll.so => 108
	i64 u0xb2a3f67f3bf29fce, ; 696: da/Microsoft.Maui.Controls.resources => 297
	i64 u0xb3874072ee0ecf8c, ; 697: Xamarin.AndroidX.VectorDrawable.Animated.dll => 274
	i64 u0xb3f0a0fcda8d3ebc, ; 698: Xamarin.AndroidX.CardView => 216
	i64 u0xb46be1aa6d4fff93, ; 699: hi/Microsoft.Maui.Controls.resources => 304
	i64 u0xb477491be13109d8, ; 700: ar/Microsoft.Maui.Controls.resources => 294
	i64 u0xb4bd7015ecee9d86, ; 701: System.IO.Pipelines => 54
	i64 u0xb4c53d9749c5f226, ; 702: lib_System.IO.FileSystem.AccessControl.dll.so => 47
	i64 u0xb4ff710863453fda, ; 703: System.Diagnostics.FileVersionInfo.dll => 28
	i64 u0xb54092076b15e062, ; 704: System.Threading.AccessControl => 142
	i64 u0xb5c38bf497a4cfe2, ; 705: lib_System.Threading.Tasks.dll.so => 148
	i64 u0xb5c7fcdafbc67ee4, ; 706: Microsoft.Extensions.Logging.Abstractions.dll => 188
	i64 u0xb5ea31d5244c6626, ; 707: System.Threading.ThreadPool.dll => 150
	i64 u0xb7212c4683a94afe, ; 708: System.Drawing.Primitives => 35
	i64 u0xb7b7753d1f319409, ; 709: sv/Microsoft.Maui.Controls.resources => 320
	i64 u0xb81a2c6e0aee50fe, ; 710: lib_System.Private.CoreLib.dll.so => 177
	i64 u0xb8c60af47c08d4da, ; 711: System.Net.ServicePoint => 77
	i64 u0xb8e68d20aad91196, ; 712: lib_System.Xml.XPath.dll.so => 164
	i64 u0xb9185c33a1643eed, ; 713: Microsoft.CSharp.dll => 1
	i64 u0xb960d6b2200ba320, ; 714: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android.dll => 252
	i64 u0xb9b8001adf4ed7cc, ; 715: lib_Xamarin.AndroidX.SlidingPaneLayout.dll.so => 267
	i64 u0xb9f64d3b230def68, ; 716: lib-pt-Microsoft.Maui.Controls.resources.dll.so => 316
	i64 u0xb9fc3c8a556e3691, ; 717: ja/Microsoft.Maui.Controls.resources => 309
	i64 u0xba4670aa94a2b3c6, ; 718: lib_System.Xml.XDocument.dll.so => 162
	i64 u0xba48785529705af9, ; 719: System.Collections.dll => 12
	i64 u0xba965b8c86359996, ; 720: lib_System.Windows.dll.so => 158
	i64 u0xbb286883bc35db36, ; 721: System.Transactions.dll => 154
	i64 u0xbb65706fde942ce3, ; 722: System.Net.Sockets => 78
	i64 u0xbba28979413cad9e, ; 723: lib_System.Runtime.CompilerServices.VisualC.dll.so => 105
	i64 u0xbbd180354b67271a, ; 724: System.Runtime.Serialization.Formatters => 114
	i64 u0xbc22a245dab70cb4, ; 725: lib_SQLitePCLRaw.provider.e_sqlite3.dll.so => 201
	i64 u0xbc260cdba33291a3, ; 726: Xamarin.AndroidX.Arch.Core.Common.dll => 213
	i64 u0xbd0e2c0d55246576, ; 727: System.Net.Http.dll => 66
	i64 u0xbd3fbd85b9e1cb29, ; 728: lib_System.Net.HttpListener.dll.so => 67
	i64 u0xbd437a2cdb333d0d, ; 729: Xamarin.AndroidX.ViewPager2 => 277
	i64 u0xbd4f572d2bd0a789, ; 730: System.IO.Compression.ZipFile.dll => 45
	i64 u0xbd5d0b88d3d647a5, ; 731: lib_Xamarin.AndroidX.Browser.dll.so => 215
	i64 u0xbd877b14d0b56392, ; 732: System.Runtime.Intrinsics.dll => 111
	i64 u0xbe65a49036345cf4, ; 733: lib_System.Buffers.dll.so => 7
	i64 u0xbee38d4a88835966, ; 734: Xamarin.AndroidX.AppCompat.AppCompatResources => 212
	i64 u0xbef9919db45b4ca7, ; 735: System.IO.Pipes.AccessControl => 55
	i64 u0xbf0fa68611139208, ; 736: lib_Xamarin.AndroidX.Annotation.dll.so => 208
	i64 u0xbfc1e1fb3095f2b3, ; 737: lib_System.Net.Http.Json.dll.so => 65
	i64 u0xc040a4ab55817f58, ; 738: ar/Microsoft.Maui.Controls.resources.dll => 294
	i64 u0xc07cadab29efeba0, ; 739: Xamarin.AndroidX.Core.Core.Ktx.dll => 225
	i64 u0xc0d928351ab5ca77, ; 740: System.Console.dll => 20
	i64 u0xc0f5a221a9383aea, ; 741: System.Runtime.Intrinsics => 111
	i64 u0xc111030af54d7191, ; 742: System.Resources.Writer => 103
	i64 u0xc12b8b3afa48329c, ; 743: lib_System.Linq.dll.so => 63
	i64 u0xc183ca0b74453aa9, ; 744: lib_System.Threading.Tasks.Dataflow.dll.so => 145
	i64 u0xc1ff9ae3cdb6e1e6, ; 745: Xamarin.AndroidX.Activity.dll => 206
	i64 u0xc26c064effb1dea9, ; 746: System.Buffers.dll => 7
	i64 u0xc28c50f32f81cc73, ; 747: ja/Microsoft.Maui.Controls.resources.dll => 309
	i64 u0xc2902f6cf5452577, ; 748: lib_Mono.Android.Export.dll.so => 173
	i64 u0xc2a3bca55b573141, ; 749: System.IO.FileSystem.Watcher => 50
	i64 u0xc2bcfec99f69365e, ; 750: Xamarin.AndroidX.ViewPager2.dll => 277
	i64 u0xc30b52815b58ac2c, ; 751: lib_System.Runtime.Serialization.Xml.dll.so => 117
	i64 u0xc36d7d89c652f455, ; 752: System.Threading.Overlapped => 144
	i64 u0xc396b285e59e5493, ; 753: GoogleGson.dll => 179
	i64 u0xc3c86c1e5e12f03d, ; 754: WindowsBase => 169
	i64 u0xc421b61fd853169d, ; 755: lib_System.Net.WebSockets.Client.dll.so => 82
	i64 u0xc463e077917aa21d, ; 756: System.Runtime.Serialization.Json => 115
	i64 u0xc4d3858ed4d08512, ; 757: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 251
	i64 u0xc50fded0ded1418c, ; 758: lib_System.ComponentModel.TypeConverter.dll.so => 17
	i64 u0xc519125d6bc8fb11, ; 759: lib_System.Net.Requests.dll.so => 74
	i64 u0xc5293b19e4dc230e, ; 760: Xamarin.AndroidX.Navigation.Fragment => 256
	i64 u0xc5325b2fcb37446f, ; 761: lib_System.Private.Xml.dll.so => 91
	i64 u0xc535cb9a21385d9b, ; 762: lib_Xamarin.Android.Glide.DiskLruCache.dll.so => 204
	i64 u0xc5a0f4b95a699af7, ; 763: lib_System.Private.Uri.dll.so => 89
	i64 u0xc5cdcd5b6277579e, ; 764: lib_System.Security.Cryptography.Algorithms.dll.so => 122
	i64 u0xc5ec286825cb0bf4, ; 765: Xamarin.AndroidX.Tracing.Tracing => 270
	i64 u0xc6706bc8aa7fe265, ; 766: Xamarin.AndroidX.Annotation.Jvm => 210
	i64 u0xc74d70d4aa96cef3, ; 767: Xamarin.AndroidX.Navigation.Runtime.Android => 258
	i64 u0xc7c01e7d7c93a110, ; 768: System.Text.Encoding.Extensions.dll => 137
	i64 u0xc7ce851898a4548e, ; 769: lib_System.Web.HttpUtility.dll.so => 156
	i64 u0xc809d4089d2556b2, ; 770: System.Runtime.InteropServices.JavaScript.dll => 108
	i64 u0xc858a28d9ee5a6c5, ; 771: lib_System.Collections.Specialized.dll.so => 11
	i64 u0xc8ac7c6bf1c2ec51, ; 772: System.Reflection.DispatchProxy.dll => 92
	i64 u0xc9c62c8f354ac568, ; 773: lib_System.Diagnostics.TextWriterTraceListener.dll.so => 31
	i64 u0xca3a723e7342c5b6, ; 774: lib-tr-Microsoft.Maui.Controls.resources.dll.so => 322
	i64 u0xca5801070d9fccfb, ; 775: System.Text.Encoding => 138
	i64 u0xcab3493c70141c2d, ; 776: pl/Microsoft.Maui.Controls.resources => 314
	i64 u0xcacfddc9f7c6de76, ; 777: ro/Microsoft.Maui.Controls.resources.dll => 317
	i64 u0xcadbc92899a777f0, ; 778: Xamarin.AndroidX.Startup.StartupRuntime => 268
	i64 u0xcba1cb79f45292b5, ; 779: Xamarin.Android.Glide.GifDecoder.dll => 205
	i64 u0xcbb5f80c7293e696, ; 780: lib_System.Globalization.Calendars.dll.so => 40
	i64 u0xcbd4fdd9cef4a294, ; 781: lib__Microsoft.Android.Resource.Designer.dll.so => 328
	i64 u0xcc15da1e07bbd994, ; 782: Xamarin.AndroidX.SlidingPaneLayout => 267
	i64 u0xcc2876b32ef2794c, ; 783: lib_System.Text.RegularExpressions.dll.so => 141
	i64 u0xcc5c3bb714c4561e, ; 784: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 291
	i64 u0xcc76886e09b88260, ; 785: Xamarin.KotlinX.Serialization.Core.Jvm.dll => 293
	i64 u0xcc9fa2923aa1c9ef, ; 786: System.Diagnostics.Contracts.dll => 25
	i64 u0xccae9bb73e2326bd, ; 787: lib_System.IO.Hashing.dll.so => 176
	i64 u0xccf25c4b634ccd3a, ; 788: zh-Hans/Microsoft.Maui.Controls.resources.dll => 326
	i64 u0xcd10a42808629144, ; 789: System.Net.Requests => 74
	i64 u0xcdca1b920e9f53ba, ; 790: Xamarin.AndroidX.Interpolator => 237
	i64 u0xcdd0c48b6937b21c, ; 791: Xamarin.AndroidX.SwipeRefreshLayout => 269
	i64 u0xcf23d8093f3ceadf, ; 792: System.Diagnostics.DiagnosticSource.dll => 27
	i64 u0xcf5ff6b6b2c4c382, ; 793: System.Net.Mail.dll => 68
	i64 u0xcf8fc898f98b0d34, ; 794: System.Private.Xml.Linq => 90
	i64 u0xd04b5f59ed596e31, ; 795: System.Reflection.Metadata.dll => 97
	i64 u0xd063299fcfc0c93f, ; 796: lib_System.Runtime.Serialization.Json.dll.so => 115
	i64 u0xd0de8a113e976700, ; 797: System.Diagnostics.TextWriterTraceListener => 31
	i64 u0xd0fc33d5ae5d4cb8, ; 798: System.Runtime.Extensions => 106
	i64 u0xd1194e1d8a8de83c, ; 799: lib_Xamarin.AndroidX.Lifecycle.Common.Jvm.dll.so => 239
	i64 u0xd12beacdfc14f696, ; 800: System.Dynamic.Runtime => 37
	i64 u0xd16fd7fb9bbcd43e, ; 801: Microsoft.Extensions.Diagnostics.Abstractions => 184
	i64 u0xd198e7ce1b6a8344, ; 802: System.Net.Quic.dll => 73
	i64 u0xd3144156a3727ebe, ; 803: Xamarin.Google.Guava.ListenableFuture => 285
	i64 u0xd333d0af9e423810, ; 804: System.Runtime.InteropServices => 110
	i64 u0xd33a415cb4278969, ; 805: System.Security.Cryptography.Encoding.dll => 125
	i64 u0xd3426d966bb704f5, ; 806: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 212
	i64 u0xd3651b6fc3125825, ; 807: System.Private.Uri.dll => 89
	i64 u0xd373685349b1fe8b, ; 808: Microsoft.Extensions.Logging.dll => 187
	i64 u0xd3801faafafb7698, ; 809: System.Private.DataContractSerialization.dll => 88
	i64 u0xd3e4c8d6a2d5d470, ; 810: it/Microsoft.Maui.Controls.resources => 308
	i64 u0xd3edcc1f25459a50, ; 811: System.Reflection.Emit => 95
	i64 u0xd4645626dffec99d, ; 812: lib_Microsoft.Extensions.DependencyInjection.Abstractions.dll.so => 183
	i64 u0xd4fa0abb79079ea9, ; 813: System.Security.Principal.dll => 131
	i64 u0xd5507e11a2b2839f, ; 814: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 251
	i64 u0xd5d04bef8478ea19, ; 815: Xamarin.AndroidX.Tracing.Tracing.dll => 270
	i64 u0xd60815f26a12e140, ; 816: Microsoft.Extensions.Logging.Debug.dll => 189
	i64 u0xd6694f8359737e4e, ; 817: Xamarin.AndroidX.SavedState => 263
	i64 u0xd6949e129339eae5, ; 818: lib_Xamarin.AndroidX.Core.Core.Ktx.dll.so => 225
	i64 u0xd6d21782156bc35b, ; 819: Xamarin.AndroidX.SwipeRefreshLayout.dll => 269
	i64 u0xd6de019f6af72435, ; 820: Xamarin.AndroidX.ConstraintLayout.Core.dll => 222
	i64 u0xd70956d1e6deefb9, ; 821: Jsr305Binding => 282
	i64 u0xd72329819cbbbc44, ; 822: lib_Microsoft.Extensions.Configuration.Abstractions.dll.so => 181
	i64 u0xd72c760af136e863, ; 823: System.Xml.XmlSerializer.dll => 166
	i64 u0xd753f071e44c2a03, ; 824: lib_System.Security.SecureString.dll.so => 132
	i64 u0xd7b3764ada9d341d, ; 825: lib_Microsoft.Extensions.Logging.Abstractions.dll.so => 188
	i64 u0xd7f0088bc5ad71f2, ; 826: Xamarin.AndroidX.VersionedParcelable => 275
	i64 u0xd824ef6ab33f8f7a, ; 827: Xamarin.AndroidX.Window.WindowCore.dll => 279
	i64 u0xd8fb25e28ae30a12, ; 828: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll => 260
	i64 u0xda1dfa4c534a9251, ; 829: Microsoft.Extensions.DependencyInjection => 182
	i64 u0xdad05a11827959a3, ; 830: System.Collections.NonGeneric.dll => 10
	i64 u0xdaefdfe71aa53cf9, ; 831: System.IO.FileSystem.Primitives => 49
	i64 u0xdb5383ab5865c007, ; 832: lib-vi-Microsoft.Maui.Controls.resources.dll.so => 324
	i64 u0xdb58816721c02a59, ; 833: lib_System.Reflection.Emit.ILGeneration.dll.so => 93
	i64 u0xdbeda89f832aa805, ; 834: vi/Microsoft.Maui.Controls.resources.dll => 324
	i64 u0xdbf2a779fbc3ac31, ; 835: System.Transactions.Local.dll => 153
	i64 u0xdbf9607a441b4505, ; 836: System.Linq => 63
	i64 u0xdbfc90157a0de9b0, ; 837: lib_System.Text.Encoding.dll.so => 138
	i64 u0xdc75032002d1a212, ; 838: lib_System.Transactions.Local.dll.so => 153
	i64 u0xdca8be7403f92d4f, ; 839: lib_System.Linq.Queryable.dll.so => 62
	i64 u0xdce2c53525640bf3, ; 840: Microsoft.Extensions.Logging => 187
	i64 u0xdd2b722d78ef5f43, ; 841: System.Runtime.dll => 119
	i64 u0xdd67031857c72f96, ; 842: lib_System.Text.Encodings.Web.dll.so => 139
	i64 u0xdd70765ad6162057, ; 843: Xamarin.JSpecify => 287
	i64 u0xdd92e229ad292030, ; 844: System.Numerics.dll => 86
	i64 u0xdde30e6b77aa6f6c, ; 845: lib-zh-Hans-Microsoft.Maui.Controls.resources.dll.so => 326
	i64 u0xde110ae80fa7c2e2, ; 846: System.Xml.XDocument.dll => 162
	i64 u0xde4726fcdf63a198, ; 847: Xamarin.AndroidX.Transition => 272
	i64 u0xde572c2b2fb32f93, ; 848: lib_System.Threading.Tasks.Extensions.dll.so => 146
	i64 u0xde8769ebda7d8647, ; 849: hr/Microsoft.Maui.Controls.resources.dll => 305
	i64 u0xdee075f3477ef6be, ; 850: Xamarin.AndroidX.ExifInterface.dll => 234
	i64 u0xdf4b773de8fb1540, ; 851: System.Net.dll => 84
	i64 u0xdf9c7682560a9629, ; 852: System.Net.ServerSentEvents => 76
	i64 u0xdfa254ebb4346068, ; 853: System.Net.Ping => 71
	i64 u0xe0142572c095a480, ; 854: Xamarin.AndroidX.AppCompat.dll => 211
	i64 u0xe021eaa401792a05, ; 855: System.Text.Encoding.dll => 138
	i64 u0xe02f89350ec78051, ; 856: Xamarin.AndroidX.CoordinatorLayout.dll => 223
	i64 u0xe0496b9d65ef5474, ; 857: Xamarin.Android.Glide.DiskLruCache.dll => 204
	i64 u0xe10b760bb1462e7a, ; 858: lib_System.Security.Cryptography.Primitives.dll.so => 127
	i64 u0xe192a588d4410686, ; 859: lib_System.IO.Pipelines.dll.so => 54
	i64 u0xe1a08bd3fa539e0d, ; 860: System.Runtime.Loader => 112
	i64 u0xe1a77eb8831f7741, ; 861: System.Security.SecureString.dll => 132
	i64 u0xe1b52f9f816c70ef, ; 862: System.Private.Xml.Linq.dll => 90
	i64 u0xe1e199c8ab02e356, ; 863: System.Data.DataSetExtensions.dll => 23
	i64 u0xe1ecfdb7fff86067, ; 864: System.Net.Security.dll => 75
	i64 u0xe2252a80fe853de4, ; 865: lib_System.Security.Principal.dll.so => 131
	i64 u0xe22fa4c9c645db62, ; 866: System.Diagnostics.TextWriterTraceListener.dll => 31
	i64 u0xe24095a7afddaab3, ; 867: lib_Microsoft.Extensions.Hosting.Abstractions.dll.so => 186
	i64 u0xe2420585aeceb728, ; 868: System.Net.Requests.dll => 74
	i64 u0xe26692647e6bcb62, ; 869: Xamarin.AndroidX.Lifecycle.Runtime.Ktx => 246
	i64 u0xe29b73bc11392966, ; 870: lib-id-Microsoft.Maui.Controls.resources.dll.so => 307
	i64 u0xe2ad448dee50fbdf, ; 871: System.Xml.Serialization => 161
	i64 u0xe2d920f978f5d85c, ; 872: System.Data.DataSetExtensions => 23
	i64 u0xe2e426c7714fa0bc, ; 873: Microsoft.Win32.Primitives.dll => 4
	i64 u0xe332bacb3eb4a806, ; 874: Mono.Android.Export.dll => 173
	i64 u0xe3811d68d4fe8463, ; 875: pt-BR/Microsoft.Maui.Controls.resources.dll => 315
	i64 u0xe3a586956771a0ed, ; 876: lib_SQLite-net.dll.so => 197
	i64 u0xe3b7cbae5ad66c75, ; 877: lib_System.Security.Cryptography.Encoding.dll.so => 125
	i64 u0xe4292b48f3224d5b, ; 878: lib_Xamarin.AndroidX.Core.ViewTree.dll.so => 226
	i64 u0xe494f7ced4ecd10a, ; 879: hu/Microsoft.Maui.Controls.resources.dll => 306
	i64 u0xe4a9b1e40d1e8917, ; 880: lib-fi-Microsoft.Maui.Controls.resources.dll.so => 301
	i64 u0xe4f74a0b5bf9703f, ; 881: System.Runtime.Serialization.Primitives => 116
	i64 u0xe5434e8a119ceb69, ; 882: lib_Mono.Android.dll.so => 175
	i64 u0xe55703b9ce5c038a, ; 883: System.Diagnostics.Tools => 32
	i64 u0xe57013c8afc270b5, ; 884: Microsoft.VisualBasic => 3
	i64 u0xe62913cc36bc07ec, ; 885: System.Xml.dll => 167
	i64 u0xe7bea09c4900a191, ; 886: Xamarin.AndroidX.VectorDrawable.dll => 273
	i64 u0xe7e03cc18dcdeb49, ; 887: lib_System.Diagnostics.StackTrace.dll.so => 30
	i64 u0xe7e147ff99a7a380, ; 888: lib_System.Configuration.dll.so => 19
	i64 u0xe86b0df4ba9e5db8, ; 889: lib_Xamarin.AndroidX.Lifecycle.Runtime.Android.dll.so => 245
	i64 u0xe896622fe0902957, ; 890: System.Reflection.Emit.dll => 95
	i64 u0xe89a2a9ef110899b, ; 891: System.Drawing.dll => 36
	i64 u0xe8c5f8c100b5934b, ; 892: Microsoft.Win32.Registry => 5
	i64 u0xe98163eb702ae5c5, ; 893: Xamarin.AndroidX.Arch.Core.Runtime => 214
	i64 u0xe994f23ba4c143e5, ; 894: Xamarin.KotlinX.Coroutines.Android => 289
	i64 u0xe9b9c8c0458fd92a, ; 895: System.Windows => 158
	i64 u0xe9d166d87a7f2bdb, ; 896: lib_Xamarin.AndroidX.Startup.StartupRuntime.dll.so => 268
	i64 u0xe9ef9a2af61fc03f, ; 897: lib_App_v3.dll.so => 0
	i64 u0xea5a4efc2ad81d1b, ; 898: Xamarin.Google.ErrorProne.Annotations => 284
	i64 u0xeb2313fe9d65b785, ; 899: Xamarin.AndroidX.ConstraintLayout.dll => 221
	i64 u0xed19c616b3fcb7eb, ; 900: Xamarin.AndroidX.VersionedParcelable.dll => 275
	i64 u0xedc4817167106c23, ; 901: System.Net.Sockets.dll => 78
	i64 u0xedc632067fb20ff3, ; 902: System.Memory.dll => 64
	i64 u0xedc8e4ca71a02a8b, ; 903: Xamarin.AndroidX.Navigation.Runtime.dll => 257
	i64 u0xee81f5b3f1c4f83b, ; 904: System.Threading.ThreadPool => 150
	i64 u0xeeb7ebb80150501b, ; 905: lib_Xamarin.AndroidX.Collection.Jvm.dll.so => 218
	i64 u0xeefc635595ef57f0, ; 906: System.Security.Cryptography.Cng => 123
	i64 u0xef03b1b5a04e9709, ; 907: System.Text.Encoding.CodePages.dll => 136
	i64 u0xef5bcbe61622ee5f, ; 908: Xamarin.AndroidX.Tracing.Tracing.Android.dll => 271
	i64 u0xef602c523fe2e87a, ; 909: lib_Xamarin.Google.Guava.ListenableFuture.dll.so => 285
	i64 u0xef72742e1bcca27a, ; 910: Microsoft.Maui.Essentials.dll => 195
	i64 u0xefd1e0c4e5c9b371, ; 911: System.Resources.ResourceManager.dll => 102
	i64 u0xefe8f8d5ed3c72ea, ; 912: System.Formats.Tar.dll => 39
	i64 u0xefec0b7fdc57ec42, ; 913: Xamarin.AndroidX.Activity => 206
	i64 u0xeff59cbde4363ec3, ; 914: System.Threading.AccessControl.dll => 142
	i64 u0xf00c29406ea45e19, ; 915: es/Microsoft.Maui.Controls.resources.dll => 300
	i64 u0xf09e47b6ae914f6e, ; 916: System.Net.NameResolution => 69
	i64 u0xf0ac2b489fed2e35, ; 917: lib_System.Diagnostics.Debug.dll.so => 26
	i64 u0xf0bb49dadd3a1fe1, ; 918: lib_System.Net.ServicePoint.dll.so => 77
	i64 u0xf0c16dff90fbf5d6, ; 919: Xamarin.AndroidX.Window.WindowCore.Jvm => 280
	i64 u0xf0de2537ee19c6ca, ; 920: lib_System.Net.WebHeaderCollection.dll.so => 80
	i64 u0xf1138779fa181c68, ; 921: lib_Xamarin.AndroidX.Lifecycle.Runtime.dll.so => 244
	i64 u0xf11b621fc87b983f, ; 922: Microsoft.Maui.Controls.Xaml.dll => 193
	i64 u0xf161f4f3c3b7e62c, ; 923: System.Data => 24
	i64 u0xf16eb650d5a464bc, ; 924: System.ValueTuple => 155
	i64 u0xf1c4b4005493d871, ; 925: System.Formats.Asn1.dll => 38
	i64 u0xf22514cfad2d598b, ; 926: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android.dll.so => 252
	i64 u0xf238bd79489d3a96, ; 927: lib-nl-Microsoft.Maui.Controls.resources.dll.so => 313
	i64 u0xf2feea356ba760af, ; 928: Xamarin.AndroidX.Arch.Core.Runtime.dll => 214
	i64 u0xf300e085f8acd238, ; 929: lib_System.ServiceProcess.dll.so => 135
	i64 u0xf34e52b26e7e059d, ; 930: System.Runtime.CompilerServices.VisualC.dll => 105
	i64 u0xf37221fda4ef8830, ; 931: lib_Xamarin.Google.Android.Material.dll.so => 281
	i64 u0xf3ad9b8fb3eefd12, ; 932: lib_System.IO.UnmanagedMemoryStream.dll.so => 57
	i64 u0xf3ddfe05336abf29, ; 933: System => 168
	i64 u0xf408654b2a135055, ; 934: System.Reflection.Emit.ILGeneration.dll => 93
	i64 u0xf4103170a1de5bd0, ; 935: System.Linq.Queryable.dll => 62
	i64 u0xf42d20c23173d77c, ; 936: lib_System.ServiceModel.Web.dll.so => 134
	i64 u0xf4c1dd70a5496a17, ; 937: System.IO.Compression => 46
	i64 u0xf4ecf4b9afc64781, ; 938: System.ServiceProcess.dll => 135
	i64 u0xf4eeeaa566e9b970, ; 939: lib_Xamarin.AndroidX.CustomView.PoolingContainer.dll.so => 229
	i64 u0xf518f63ead11fcd1, ; 940: System.Threading.Tasks => 148
	i64 u0xf5fc7602fe27b333, ; 941: System.Net.WebHeaderCollection => 80
	i64 u0xf6077741019d7428, ; 942: Xamarin.AndroidX.CoordinatorLayout => 223
	i64 u0xf6742cbf457c450b, ; 943: Xamarin.AndroidX.Lifecycle.Runtime.Android.dll => 245
	i64 u0xf6e8de2aebcbb422, ; 944: lib_Xamarin.AndroidX.Window.WindowCore.Jvm.dll.so => 280
	i64 u0xf70c0a7bf8ccf5af, ; 945: System.Web => 157
	i64 u0xf77b20923f07c667, ; 946: de/Microsoft.Maui.Controls.resources.dll => 298
	i64 u0xf7e2cac4c45067b3, ; 947: lib_System.Numerics.Vectors.dll.so => 85
	i64 u0xf7e74930e0e3d214, ; 948: zh-HK/Microsoft.Maui.Controls.resources.dll => 325
	i64 u0xf84773b5c81e3cef, ; 949: lib-uk-Microsoft.Maui.Controls.resources.dll.so => 323
	i64 u0xf8aac5ea82de1348, ; 950: System.Linq.Queryable => 62
	i64 u0xf8b77539b362d3ba, ; 951: lib_System.Reflection.Primitives.dll.so => 98
	i64 u0xf8e045dc345b2ea3, ; 952: lib_Xamarin.AndroidX.RecyclerView.dll.so => 261
	i64 u0xf915dc29808193a1, ; 953: System.Web.HttpUtility.dll => 156
	i64 u0xf96c777a2a0686f4, ; 954: hi/Microsoft.Maui.Controls.resources.dll => 304
	i64 u0xf9be54c8bcf8ff3b, ; 955: System.Security.AccessControl.dll => 120
	i64 u0xf9eec5bb3a6aedc6, ; 956: Microsoft.Extensions.Options => 190
	i64 u0xfa0e82300e67f913, ; 957: lib_System.AppContext.dll.so => 6
	i64 u0xfa2fdb27e8a2c8e8, ; 958: System.ComponentModel.EventBasedAsync => 15
	i64 u0xfa3f278f288b0e84, ; 959: lib_System.Net.Security.dll.so => 75
	i64 u0xfa504dfa0f097d72, ; 960: Microsoft.Extensions.FileProviders.Abstractions.dll => 185
	i64 u0xfa5ed7226d978949, ; 961: lib-ar-Microsoft.Maui.Controls.resources.dll.so => 294
	i64 u0xfa645d91e9fc4cba, ; 962: System.Threading.Thread => 149
	i64 u0xfad4d2c770e827f9, ; 963: lib_System.IO.IsolatedStorage.dll.so => 52
	i64 u0xfb022853d73b7fa5, ; 964: lib_SQLitePCLRaw.batteries_v2.dll.so => 198
	i64 u0xfb06dd2338e6f7c4, ; 965: System.Net.Ping.dll => 71
	i64 u0xfb087abe5365e3b7, ; 966: lib_System.Data.DataSetExtensions.dll.so => 23
	i64 u0xfb846e949baff5ea, ; 967: System.Xml.Serialization.dll => 161
	i64 u0xfbad3e4ce4b98145, ; 968: System.Security.Cryptography.X509Certificates => 128
	i64 u0xfbf0a31c9fc34bc4, ; 969: lib_System.Net.Http.dll.so => 66
	i64 u0xfc6b7527cc280b3f, ; 970: lib_System.Runtime.Serialization.Formatters.dll.so => 114
	i64 u0xfc719aec26adf9d9, ; 971: Xamarin.AndroidX.Navigation.Fragment.dll => 256
	i64 u0xfc82690c2fe2735c, ; 972: Xamarin.AndroidX.Lifecycle.Process.dll => 243
	i64 u0xfc93fc307d279893, ; 973: System.IO.Pipes.AccessControl.dll => 55
	i64 u0xfcd302092ada6328, ; 974: System.IO.MemoryMappedFiles.dll => 53
	i64 u0xfd22f00870e40ae0, ; 975: lib_Xamarin.AndroidX.DrawerLayout.dll.so => 230
	i64 u0xfd49b3c1a76e2748, ; 976: System.Runtime.InteropServices.RuntimeInformation => 109
	i64 u0xfd536c702f64dc47, ; 977: System.Text.Encoding.Extensions => 137
	i64 u0xfd583f7657b6a1cb, ; 978: Xamarin.AndroidX.Fragment => 235
	i64 u0xfd8dd91a2c26bd5d, ; 979: Xamarin.AndroidX.Lifecycle.Runtime => 244
	i64 u0xfda36abccf05cf5c, ; 980: System.Net.WebSockets.Client => 82
	i64 u0xfddbe9695626a7f5, ; 981: Xamarin.AndroidX.Lifecycle.Common => 238
	i64 u0xfeae9952cf03b8cb, ; 982: tr/Microsoft.Maui.Controls.resources => 322
	i64 u0xfebe1950717515f9, ; 983: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll => 242
	i64 u0xff270a55858bac8d, ; 984: System.Security.Principal => 131
	i64 u0xff9b54613e0d2cc8, ; 985: System.Net.Http.Json => 65
	i64 u0xffdb7a971be4ec73 ; 986: System.ValueTuple.dll => 155
], align 8

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [987 x i32] [
	i32 42, i32 290, i32 269, i32 13, i32 257, i32 107, i32 174, i32 48,
	i32 211, i32 7, i32 201, i32 88, i32 318, i32 296, i32 324, i32 231,
	i32 72, i32 261, i32 12, i32 194, i32 104, i32 325, i32 159, i32 19,
	i32 236, i32 218, i32 164, i32 233, i32 0, i32 273, i32 170, i32 318,
	i32 10, i32 189, i32 274, i32 98, i32 229, i32 230, i32 13, i32 190,
	i32 10, i32 129, i32 97, i32 143, i32 39, i32 319, i32 293, i32 276,
	i32 315, i32 175, i32 205, i32 5, i32 195, i32 68, i32 266, i32 132,
	i32 265, i32 232, i32 69, i32 219, i32 67, i32 57, i32 228, i32 52,
	i32 43, i32 127, i32 68, i32 83, i32 246, i32 161, i32 94, i32 101,
	i32 261, i32 144, i32 154, i32 215, i32 302, i32 165, i32 172, i32 303,
	i32 183, i32 83, i32 287, i32 219, i32 4, i32 5, i32 51, i32 103,
	i32 56, i32 122, i32 100, i32 171, i32 120, i32 290, i32 21, i32 306,
	i32 139, i32 99, i32 293, i32 79, i32 312, i32 268, i32 121, i32 8,
	i32 168, i32 321, i32 71, i32 204, i32 247, i32 262, i32 174, i32 148,
	i32 40, i32 266, i32 47, i32 30, i32 259, i32 310, i32 147, i32 190,
	i32 166, i32 28, i32 86, i32 270, i32 79, i32 43, i32 29, i32 42,
	i32 105, i32 119, i32 209, i32 45, i32 93, i32 321, i32 56, i32 151,
	i32 149, i32 102, i32 49, i32 20, i32 224, i32 116, i32 202, i32 302,
	i32 283, i32 198, i32 288, i32 191, i32 96, i32 58, i32 307, i32 305,
	i32 279, i32 83, i32 283, i32 172, i32 26, i32 72, i32 260, i32 184,
	i32 234, i32 255, i32 323, i32 70, i32 33, i32 301, i32 14, i32 141,
	i32 38, i32 327, i32 220, i32 314, i32 136, i32 94, i32 90, i32 152,
	i32 280, i32 320, i32 24, i32 140, i32 57, i32 142, i32 51, i32 299,
	i32 29, i32 160, i32 34, i32 167, i32 235, i32 52, i32 328, i32 278,
	i32 92, i32 216, i32 35, i32 302, i32 160, i32 9, i32 300, i32 78,
	i32 59, i32 55, i32 194, i32 296, i32 192, i32 13, i32 277, i32 180,
	i32 213, i32 111, i32 250, i32 32, i32 106, i32 86, i32 94, i32 53,
	i32 98, i32 286, i32 58, i32 9, i32 104, i32 228, i32 69, i32 276,
	i32 295, i32 185, i32 127, i32 262, i32 118, i32 137, i32 264, i32 128,
	i32 108, i32 288, i32 133, i32 215, i32 285, i32 150, i32 159, i32 236,
	i32 224, i32 198, i32 231, i32 262, i32 99, i32 24, i32 267, i32 146,
	i32 186, i32 254, i32 3, i32 170, i32 212, i32 102, i32 164, i32 101,
	i32 226, i32 25, i32 95, i32 171, i32 175, i32 207, i32 3, i32 314,
	i32 271, i32 233, i32 1, i32 116, i32 288, i32 236, i32 243, i32 33,
	i32 6, i32 318, i32 159, i32 316, i32 53, i32 87, i32 275, i32 259,
	i32 44, i32 242, i32 106, i32 47, i32 140, i32 252, i32 65, i32 253,
	i32 70, i32 82, i32 60, i32 91, i32 157, i32 213, i32 135, i32 112,
	i32 59, i32 308, i32 253, i32 260, i32 174, i32 136, i32 143, i32 40,
	i32 295, i32 200, i32 192, i32 61, i32 178, i32 249, i32 81, i32 25,
	i32 36, i32 101, i32 246, i32 72, i32 22, i32 224, i32 196, i32 319,
	i32 123, i32 70, i32 109, i32 325, i32 121, i32 119, i32 238, i32 255,
	i32 239, i32 11, i32 2, i32 126, i32 117, i32 145, i32 41, i32 89,
	i32 208, i32 199, i32 177, i32 186, i32 27, i32 151, i32 309, i32 182,
	i32 284, i32 207, i32 1, i32 209, i32 44, i32 223, i32 152, i32 18,
	i32 88, i32 297, i32 41, i32 242, i32 217, i32 247, i32 96, i32 187,
	i32 28, i32 41, i32 80, i32 232, i32 220, i32 147, i32 110, i32 218,
	i32 11, i32 107, i32 139, i32 16, i32 124, i32 67, i32 160, i32 22,
	i32 200, i32 299, i32 292, i32 104, i32 182, i32 291, i32 64, i32 58,
	i32 193, i32 298, i32 112, i32 177, i32 258, i32 289, i32 9, i32 281,
	i32 122, i32 100, i32 107, i32 76, i32 250, i32 192, i32 113, i32 210,
	i32 49, i32 59, i32 20, i32 249, i32 227, i32 73, i32 222, i32 158,
	i32 39, i32 297, i32 35, i32 38, i32 303, i32 200, i32 110, i32 312,
	i32 21, i32 286, i32 248, i32 196, i32 15, i32 191, i32 81, i32 81,
	i32 227, i32 191, i32 256, i32 265, i32 155, i32 21, i32 194, i32 296,
	i32 50, i32 51, i32 322, i32 312, i32 96, i32 203, i32 184, i32 308,
	i32 16, i32 226, i32 125, i32 305, i32 163, i32 45, i32 284, i32 179,
	i32 118, i32 64, i32 169, i32 180, i32 14, i32 263, i32 113, i32 210,
	i32 61, i32 76, i32 123, i32 311, i32 2, i32 321, i32 235, i32 248,
	i32 287, i32 248, i32 6, i32 217, i32 301, i32 231, i32 17, i32 319,
	i32 298, i32 79, i32 221, i32 255, i32 133, i32 286, i32 311, i32 85,
	i32 189, i32 12, i32 34, i32 121, i32 292, i32 243, i32 176, i32 233,
	i32 87, i32 202, i32 18, i32 276, i32 181, i32 241, i32 73, i32 97,
	i32 201, i32 168, i32 237, i32 84, i32 327, i32 211, i32 216, i32 157,
	i32 36, i32 154, i32 323, i32 197, i32 326, i32 147, i32 56, i32 115,
	i32 217, i32 273, i32 272, i32 37, i32 327, i32 180, i32 117, i32 209,
	i32 14, i32 203, i32 149, i32 43, i32 195, i32 207, i32 100, i32 291,
	i32 171, i32 16, i32 264, i32 48, i32 109, i32 99, i32 253, i32 27,
	i32 130, i32 29, i32 303, i32 185, i32 265, i32 130, i32 44, i32 227,
	i32 232, i32 152, i32 8, i32 271, i32 254, i32 304, i32 317, i32 199,
	i32 316, i32 134, i32 315, i32 42, i32 292, i32 199, i32 33, i32 328,
	i32 46, i32 146, i32 249, i32 193, i32 240, i32 228, i32 140, i32 63,
	i32 134, i32 295, i32 48, i32 163, i32 214, i32 240, i32 203, i32 238,
	i32 311, i32 272, i32 46, i32 167, i32 176, i32 237, i32 300, i32 234,
	i32 307, i32 196, i32 18, i32 8, i32 179, i32 225, i32 264, i32 126,
	i32 60, i32 144, i32 178, i32 257, i32 310, i32 244, i32 282, i32 278,
	i32 153, i32 145, i32 290, i32 128, i32 289, i32 163, i32 165, i32 229,
	i32 206, i32 181, i32 258, i32 313, i32 26, i32 254, i32 241, i32 84,
	i32 278, i32 129, i32 283, i32 103, i32 151, i32 281, i32 279, i32 259,
	i32 54, i32 165, i32 170, i32 133, i32 37, i32 274, i32 310, i32 178,
	i32 22, i32 114, i32 92, i32 50, i32 61, i32 124, i32 85, i32 129,
	i32 166, i32 282, i32 169, i32 263, i32 266, i32 230, i32 202, i32 245,
	i32 4, i32 239, i32 306, i32 173, i32 2, i32 250, i32 0, i32 118,
	i32 208, i32 19, i32 188, i32 91, i32 66, i32 30, i32 183, i32 299,
	i32 222, i32 60, i32 113, i32 241, i32 32, i32 130, i32 162, i32 317,
	i32 220, i32 143, i32 313, i32 156, i32 17, i32 219, i32 205, i32 77,
	i32 75, i32 15, i32 172, i32 87, i32 197, i32 126, i32 240, i32 251,
	i32 221, i32 320, i32 247, i32 34, i32 120, i32 141, i32 124, i32 108,
	i32 297, i32 274, i32 216, i32 304, i32 294, i32 54, i32 47, i32 28,
	i32 142, i32 148, i32 188, i32 150, i32 35, i32 320, i32 177, i32 77,
	i32 164, i32 1, i32 252, i32 267, i32 316, i32 309, i32 162, i32 12,
	i32 158, i32 154, i32 78, i32 105, i32 114, i32 201, i32 213, i32 66,
	i32 67, i32 277, i32 45, i32 215, i32 111, i32 7, i32 212, i32 55,
	i32 208, i32 65, i32 294, i32 225, i32 20, i32 111, i32 103, i32 63,
	i32 145, i32 206, i32 7, i32 309, i32 173, i32 50, i32 277, i32 117,
	i32 144, i32 179, i32 169, i32 82, i32 115, i32 251, i32 17, i32 74,
	i32 256, i32 91, i32 204, i32 89, i32 122, i32 270, i32 210, i32 258,
	i32 137, i32 156, i32 108, i32 11, i32 92, i32 31, i32 322, i32 138,
	i32 314, i32 317, i32 268, i32 205, i32 40, i32 328, i32 267, i32 141,
	i32 291, i32 293, i32 25, i32 176, i32 326, i32 74, i32 237, i32 269,
	i32 27, i32 68, i32 90, i32 97, i32 115, i32 31, i32 106, i32 239,
	i32 37, i32 184, i32 73, i32 285, i32 110, i32 125, i32 212, i32 89,
	i32 187, i32 88, i32 308, i32 95, i32 183, i32 131, i32 251, i32 270,
	i32 189, i32 263, i32 225, i32 269, i32 222, i32 282, i32 181, i32 166,
	i32 132, i32 188, i32 275, i32 279, i32 260, i32 182, i32 10, i32 49,
	i32 324, i32 93, i32 324, i32 153, i32 63, i32 138, i32 153, i32 62,
	i32 187, i32 119, i32 139, i32 287, i32 86, i32 326, i32 162, i32 272,
	i32 146, i32 305, i32 234, i32 84, i32 76, i32 71, i32 211, i32 138,
	i32 223, i32 204, i32 127, i32 54, i32 112, i32 132, i32 90, i32 23,
	i32 75, i32 131, i32 31, i32 186, i32 74, i32 246, i32 307, i32 161,
	i32 23, i32 4, i32 173, i32 315, i32 197, i32 125, i32 226, i32 306,
	i32 301, i32 116, i32 175, i32 32, i32 3, i32 167, i32 273, i32 30,
	i32 19, i32 245, i32 95, i32 36, i32 5, i32 214, i32 289, i32 158,
	i32 268, i32 0, i32 284, i32 221, i32 275, i32 78, i32 64, i32 257,
	i32 150, i32 218, i32 123, i32 136, i32 271, i32 285, i32 195, i32 102,
	i32 39, i32 206, i32 142, i32 300, i32 69, i32 26, i32 77, i32 280,
	i32 80, i32 244, i32 193, i32 24, i32 155, i32 38, i32 252, i32 313,
	i32 214, i32 135, i32 105, i32 281, i32 57, i32 168, i32 93, i32 62,
	i32 134, i32 46, i32 135, i32 229, i32 148, i32 80, i32 223, i32 245,
	i32 280, i32 157, i32 298, i32 85, i32 325, i32 323, i32 62, i32 98,
	i32 261, i32 156, i32 304, i32 120, i32 190, i32 6, i32 15, i32 75,
	i32 185, i32 294, i32 149, i32 52, i32 198, i32 71, i32 23, i32 161,
	i32 128, i32 66, i32 114, i32 256, i32 243, i32 55, i32 53, i32 230,
	i32 109, i32 137, i32 235, i32 244, i32 82, i32 238, i32 322, i32 242,
	i32 131, i32 65, i32 155
], align 4

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 8

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 8

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 u0x0000000000000000, ; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 8

; Functions

; Function attributes: memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.mm.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 8, !tbaa !3
	ret void
}

; Strings
@.mm.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 1

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+fix-cortex-a53-835769,+neon,+outline-atomics,+v8a" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+fix-cortex-a53-835769,+neon,+outline-atomics,+v8a" }

; Metadata
!llvm.module.flags = !{!0, !1, !7, !8, !9, !10}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!".NET for Android remotes/origin/release/10.0.1xx @ d549e1dc4e2a083b08b4f24cb5495e81b99d79b5"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"branch-target-enforcement", i32 0}
!8 = !{i32 1, !"sign-return-address", i32 0}
!9 = !{i32 1, !"sign-return-address-all", i32 0}
!10 = !{i32 1, !"sign-return-address-with-bkey", i32 0}
