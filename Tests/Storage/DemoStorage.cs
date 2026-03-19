using System;
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoStorage : MonoBehaviour
    {
        [Serializable]
        public class SaveProfile
        {
            public string Name;
            public int Level;
            public List<int> OwnedItems;
        }

        public enum SaveMode
        {
            Casual = 1,
            Hardcore = 2
        }

        private void Start()
        {
            FF8.Storage.Configure(new StorageManager.Settings
            {
                location = StorageManager.Location.File,
                directory = StorageManager.Directory.PersistentDataPath,
                defaultFilePath = "Save/PlayerData.json",
                compressionType = StorageManager.CompressionType.Gzip,
                encryption = new Util.OptimizedAES(key: "AES_Key", iv: null)
            });

            FF8.Storage.SetUser("User_10001");

            DemoPrimitiveTypes();
            DemoObjectAndCollections();
            DemoUnityTypes();

            FF8.Storage.Save();
            FF8.Storage.Save("Save/PlayerData.backup.json");
            FF8.Storage.Remove("temp_value");
            FF8.Storage.Remove("coins", filePath: "Save/PlayerData.backup.json");
        }

        private void DemoPrimitiveTypes()
        {
            FF8.Storage.SetString("nickname", "F8Player", user: true);
            FF8.Storage.SetInt("coins", 2560);
            FF8.Storage.SetFloat("music_volume", 0.75f);
            FF8.Storage.SetBool("guide_finished", true);
            FF8.Storage.SetChar("grade", 'S');
            FF8.Storage.SetByte("byte_value", 128);
            FF8.Storage.SetShort("short_value", 1024);
            FF8.Storage.SetLong("long_value", 9876543210L);
            FF8.Storage.SetDouble("double_value", 12.3456789d);
            FF8.Storage.SetDecimal("decimal_value", 99.99m);
            FF8.Storage.SetSByte("sbyte_value", -12);
            FF8.Storage.SetUShort("ushort_value", 65530);
            FF8.Storage.SetUInt("uint_value", 1234567890U);
            FF8.Storage.SetULong("ulong_value", 1234567890123456789UL);
            FF8.Storage.SetEnum("save_mode", SaveMode.Hardcore);

            string nickname = FF8.Storage.GetString("nickname", user: true);
            int coins = FF8.Storage.GetInt("coins");
            float volume = FF8.Storage.GetFloat("music_volume");
            bool guideFinished = FF8.Storage.GetBool("guide_finished");
            char grade = FF8.Storage.GetChar("grade");
            SaveMode saveMode = FF8.Storage.GetEnum("save_mode", SaveMode.Casual);

            LogF8.Log($"nickname={nickname}, coins={coins}, volume={volume}, guide={guideFinished}, grade={grade}, mode={saveMode}");
        }

        private void DemoObjectAndCollections()
        {
            SaveProfile profile = new SaveProfile
            {
                Name = "Knight",
                Level = 18,
                OwnedItems = new List<int> { 1001, 1002, 1003 }
            };

            FF8.Storage.SetObject("profile", profile);
            FF8.Storage.Set("int_array", new[] { 1, 2, 3, 4 });
            FF8.Storage.SetList("string_list", new List<string> { "A", "B", "C" });
            FF8.Storage.SetDictionary("map", new Dictionary<int, string>
            {
                { 1, "one" },
                { 2, "two" }
            });
            FF8.Storage.SetQueue("queue", new Queue<int>(new[] { 10, 20, 30 }));
            FF8.Storage.SetHashSet("hashset", new HashSet<string> { "fire", "ice" });
            FF8.Storage.SetStack("stack", new Stack<int>(new[] { 7, 8, 9 }));
            FF8.Storage.SetRectangularArray("grid", new int[,]
            {
                { 1, 2 },
                { 3, 4 }
            });
            FF8.Storage.SetJaggedArray("jagged", new int[][]
            {
                new[] { 1, 2 },
                new[] { 3, 4, 5 }
            });

            FF8.Storage.SetValueTuple("tuple2", (1, "tuple"));
            FF8.Storage.SetValueTuple("tuple7", (1, 2, 3, 4, 5, 6, 7));

            SaveProfile profileResult = FF8.Storage.GetObject<SaveProfile>("profile");
            int[] arrayResult = FF8.Storage.Get<int[]>("int_array");
            Dictionary<int, string> mapResult = FF8.Storage.GetDictionary<int, string>("map");
            Queue<int> queueResult = FF8.Storage.GetQueue<int>("queue");
            (int, string) tuple2Result = FF8.Storage.GetValueTuple<int, string>("tuple2");
            (int, int, int, int, int, int, int) tuple7Result =
                FF8.Storage.GetValueTuple<int, int, int, int, int, int, int>("tuple7");

            LogF8.Log($"profile={profileResult.Name}, arrayLength={arrayResult.Length}, mapValue={mapResult[1]}");
            LogF8.Log($"queuePeek={(queueResult.Count > 0 ? queueResult.Peek() : -1)}, tuple2={tuple2Result}, tuple7={tuple7Result}");
        }

        private void DemoUnityTypes()
        {
            FF8.Storage.SetVector2("vector2", new Vector2(1.5f, 2.5f));
            FF8.Storage.SetVector3("vector3", new Vector3(3f, 4f, 5f));
            FF8.Storage.SetVector4("vector4", new Vector4(6f, 7f, 8f, 9f));
            FF8.Storage.SetVector2Int("vector2int", new Vector2Int(10, 11));
            FF8.Storage.SetVector3Int("vector3int", new Vector3Int(12, 13, 14));
            FF8.Storage.SetQuaternion("rotation", Quaternion.Euler(15f, 30f, 45f));
            FF8.Storage.SetColor("color", new Color(0.1f, 0.2f, 0.3f, 1f));
            FF8.Storage.SetColor32("color32", new Color32(10, 20, 30, 255));
            FF8.Storage.SetMatrix4x4("matrix", Matrix4x4.TRS(Vector3.one, Quaternion.identity, Vector3.one));
            FF8.Storage.SetBounds("bounds", new Bounds(Vector3.zero, Vector3.one * 2));
            FF8.Storage.SetRect("rect", new Rect(5f, 10f, 100f, 50f));
            FF8.Storage.SetRectOffset("rect_offset", new RectOffset(1, 2, 3, 4));
            FF8.Storage.SetLayerMask("layer_mask", (LayerMask)(1 << 8));

            Vector3 vector3 = FF8.Storage.GetVector3("vector3");
            Quaternion rotation = FF8.Storage.GetQuaternion("rotation");
            Bounds bounds = FF8.Storage.GetBounds("bounds");
            Rect rect = FF8.Storage.GetRect("rect");
            LayerMask layerMask = FF8.Storage.GetLayerMask("layer_mask");

            LogF8.Log($"vector3={vector3}, rotation={rotation.eulerAngles}, bounds={bounds}");
            LogF8.Log($"rect={rect}, rectOffset={FF8.Storage.GetRectOffset("rect_offset")}, layerMask={layerMask.value}");
        }
    }
}
