using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ClemCAddons.CameraAndNodes;
using ClemCAddons.Utilities;
using System.Threading.Tasks;
using System.Diagnostics;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;

namespace ClemCAddons
{
    #region Extensions
    public static class Extensions
    {
        #region Logic
        #endregion Logic
        #region Type
        public static Type GetType(this string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
        public static Type GetTypeQualified(this string typeName, string assembly)
        {
            return Type.GetType(typeName + ", " + assembly);
        }
        #endregion Type
        #region Byte formatting
        public static dynamic ToType(this byte[] bytes, Type type)
        {
            var binformatter = new BinaryFormatter();
            var stream = new MemoryStream(bytes);
            return Convert.ChangeType(binformatter.Deserialize(stream), type);
        } // noice. Am actually impressed with it
        public static object ToObject(this byte[] bytes)
        {
            var binformatter = new BinaryFormatter();
            var stream = new MemoryStream(bytes);
            return binformatter.Deserialize(stream);
        }
        public static byte[] ToBytes(this object value)
        {
            var binformatter = new BinaryFormatter();
            var stream = new MemoryStream();
            binformatter.Serialize(stream, value);
            return stream.ToArray();
        }
        public static byte[] ToRawBytes(this string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }
        public static string ToRawString(this byte[] value)
        {
            return Encoding.ASCII.GetString(value);
        }

        public static Stream ToSerializedStream(this object value)
        {
            var binformatter = new BinaryFormatter();
            var stream = new MemoryStream();
            binformatter.Serialize(stream, value);
            return stream;
        }
        #endregion Byte formatting
        #region ArrayAdditions
        #region Remove
        public static T[] RemoveAt<T>(this T[] source, int index) // thanks stack overflow
        {
            if (index < source.Length)
            {
                T[] dest = new T[source.Length - 1];
                if (index > 0)
                    Array.Copy(source, 0, dest, 0, index);
                if (index < source.Length - 1)
                    Array.Copy(source, index + 1, dest, index, source.Length - index - 1);
                Array.Copy(dest, source, dest.Length);
                Array.Resize(ref source, dest.Length);
                return source;
            }
            Debug.LogError("index out of range");
            return source;
        }
        public static T[] RemoveAt<T>(ref T[] source, int index) // thanks stack overflow
        {
            if (index < source.Length)
            {
                T[] dest = new T[source.Length - 1];
                if (index > 0)
                    Array.Copy(source, 0, dest, 0, index);
                if (index < source.Length - 1)
                    Array.Copy(source, index + 1, dest, index, source.Length - index - 1);
                Array.Copy(dest, source, dest.Length);
                Array.Resize(ref source, dest.Length);
                return source;
            }
            Debug.LogError("index out of range");
            return source;
        }
        public static T[] RemoveAll<T>(this T[] source, T toRemove) // thanks stack overflow
        {
            if (source.Length == 0)
                return source;
            for (int i = source.Length - 1; i >= 0; i--)
            {
                if (source[i].Equals(toRemove))
                {
                    source = source.RemoveAt(i);
                }
            }
            return source;
        }
        #endregion Remove
        #region Add
        public static T[] Add<T>(this T[] source, T value)
        {
            T[] r = new T[source.Length];
            Array.Copy(source, r, source.Length);
            Array.Resize(ref r, source.Length + 1);
            r[source.Length] = value;
            return r;
        }
        public static T[] Add<T>(this T[] source, T value, int index)
        {
            List<T> r = source.ToList();
            r.Insert(index, value);
            return r.ToArray();
        }
        #endregion Add
        #region AddValue
        public static T[] AddValue<T>(this T[] source, T value)
        {
            dynamic result = source;
            for (int i = 0; i < source.Length; i++)
            {
                result[i] = result[i] + value;
            }
            return source;
        }
        public static T[] SubstractValue<T>(this T[] source, T value)
        {
            dynamic result = source;
            for (int i = 0; i < source.Length; i++)
            {
                result[i] = result[i] - value;
            }
            return source;
        }
        public static T[] MultiplyValue<T>(this T[] source, T value)
        {
            dynamic result = source;
            for (int i = 0; i < source.Length; i++)
            {
                result[i] = result[i] * value;
            }
            return source;
        }
        public static T[] DivideValue<T>(this T[] source, T value)
        {
            dynamic result = source;
            for (int i = 0; i < source.Length; i++)
            {
                result[i] = result[i] / value;
            }
            return source;
        }
        #endregion AddValue
        #region Find
        public static int FindIndex<T>(this T[] source, T value)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(source[i], value))
                {
                    return i;
                }
            }
            return -1;
        }
        public static T Find<T>(this T[] source, T value)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].Equals(value))
                {
                    return source[i];
                }
            }
            return default;
        }
        public static T Find<T>(this T[] source, T value, T defaultReturn)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].Equals(value))
                {
                    return source[i];
                }
            }
            return defaultReturn;
        }
        #endregion Find
        #region ToArray
        public static T[] MakeArray<T>(this T firstObject)
        {
            return new T[] { firstObject };
        }
        #endregion ToArray
        #region SetAt
        public static T[] SetAt<T>(this T[] source, T value, int index)
        {
            dynamic result = source;
            result[index] = value;
            return result;
        }
        public static T[] SetOrCreateAt<T>(this T[] source, T value, int index)
        {
            if (source.Length <= index)
            {
                Array.Resize(ref source, index + 1);
            }
            dynamic result = source;
            result[index] = value;
            return result;
        }
        public static T[] CreateIfNotAt<T>(this T[] source, T value, int index)
        {
            if (source.Length <= index)
            {
                Array.Resize(ref source, index + 1);
                dynamic result = source;
                result[index] = value;
                return result;
            }
            return source;
        }
        #endregion SetAt
        #endregion ArrayAdditions
        #region Dictionary Additions
        public static Dictionary<TKey, TValue> AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return dictionary;
            }
            dictionary.Add(key, value);
            return dictionary;
        }
        #endregion Dictioanry Additions
        #region CopyNode
        public static NodeContent Copy(NodeContent source)
        {
            var target = new NodeContent
            {
                isFixed = source.isFixed,
                position = source.position,
                range = source.range,
                rotation = new SerializableQuaternion(source.rotation.Value),
                rotationFixed = new SerializableQuaternion(source.rotationFixed.Value),
                type = source.type
            };
            return target;
        }
        public static TPSNodeContent Copy(TPSNodeContent source)
        {
            var target = new TPSNodeContent
            {
                offset = source.offset,
                range = source.range,
                distance = source.distance
            };
            return target;
        }
        public static NodeContent Copy(this NodeContent target, NodeContent source)
        {
            target.isFixed = source.isFixed;
            target.position = source.position;
            target.range = source.range;
            target.rotation = new SerializableQuaternion(source.rotation.Value);
            target.rotationFixed = new SerializableQuaternion(source.rotationFixed.Value);
            target.type = source.type;
            return target;
        }
        public static TPSNodeContent Copy(this TPSNodeContent target, TPSNodeContent source)
        {
            target.offset = source.offset;
            target.range = source.range;
            target.distance = source.distance;
            return target;
        }
        #endregion CopyNode
        #region Casts
        public static bool CastTo(this Vector3 from, Vector3 to, LayerMask layer, string tag, bool debug = false)
        {
            Ray ray = new Ray(from, (to - from).normalized);
            bool result = false;
            RaycastHit hit = new RaycastHit();
            var hits = Physics.SphereCastAll(ray, 0.2f, Vector3.Distance(from, to), layer);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                    {
                        result = true;
                        hit = hits[i];
                        hit.distance += 0.2f;
                    }
                }
            }
            if (!result)
            {
                hits = Physics.RaycastAll(ray, Vector3.Distance(from, to), layer);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                    {
                        result = true;
                        hit = hits[i];
                    }
                }
            }
            if (debug)
            {
                Debug.DrawLine(from, to, Color.grey);
                EditorTools.DrawLineInEditor(from, to, Color.grey);
                EditorTools.DrawSphereInEditor(result ? from + (ray.direction * hit.distance) : to, 0.05f, Color.grey);
            }
            return result;
        }

        public static bool CastTo(this Vector3 from, Vector3 to, LayerMask layer, string tag, out RaycastHit hit, bool debug = false)
        {
            Ray ray = new Ray(from, (to - from).normalized);
            bool result = false;
            hit = new RaycastHit();
            var hits = Physics.SphereCastAll(ray, 0.2f, Vector3.Distance(from, to), layer);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                    {
                        result = true;
                        hit = hits[i];
                        hit.distance += 0.2f;
                    }
                }
            }
            if (!result)
            {
                hits = Physics.RaycastAll(ray, Vector3.Distance(from, to), layer);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                    {
                        result = true;
                        hit = hits[i];
                    }
                }
            }
            if (debug)
            {
                Debug.DrawLine(from, to, Color.grey);
                EditorTools.DrawLineInEditor(from, to, Color.grey);
                EditorTools.DrawSphereInEditor(result ? from + (ray.direction * hit.distance) : to, 0.05f, Color.grey);
            }
            return result;
        }

        public static bool CastTo(this Vector3 from, Vector3 to, LayerMask layer, string tag, float radius, bool debug = false)
        {
            Ray ray = new Ray(from, (to - from).normalized);
            bool result = false;
            RaycastHit hit = new RaycastHit();
            var hits = Physics.SphereCastAll(ray, radius, Vector3.Distance(from, to), layer);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                    {
                        result = true;
                        hit = hits[i];
                        hit.distance += radius;
                    }
                }
            }
            if (!result)
            {
                hits = Physics.RaycastAll(ray, Vector3.Distance(from, to), layer);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                    {
                        result = true;
                        hit = hits[i];
                    }
                }
            }
            if (debug)
            {
                Debug.DrawLine(from, to, Color.grey);
                EditorTools.DrawLineInEditor(from, to, Color.grey);
                EditorTools.DrawSphereInEditor(result ? from + (ray.direction * hit.distance) : to, 0.05f, Color.grey);
            }
            return result;
        }

        public static bool CastTo(this Vector3 from, Vector3 to, LayerMask layer, string tag, float radius, out RaycastHit hit, bool debug = false)
        {
            Ray ray = new Ray(from, (to - from).normalized);
            bool result = false;
            hit = new RaycastHit();
            var hits = Physics.SphereCastAll(ray, radius, Vector3.Distance(from, to), layer);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                    {
                        result = true;
                        hit = hits[i];
                        hit.distance += radius;
                    }
                }
            }
            if (!result)
            {
                hits = Physics.RaycastAll(ray, Vector3.Distance(from, to), layer);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                    {
                        result = true;
                        hit = hits[i];
                    }
                }
            }
            if (debug)
            {
                Debug.DrawLine(from, to, Color.grey);
                EditorTools.DrawLineInEditor(from, to, Color.grey);
                EditorTools.DrawSphereInEditor(result ? from + (ray.direction * hit.distance) : to, 0.05f, Color.grey);
            }
            return result;
        }
        public static bool CastToLineOnly(this Vector3 from, Vector3 to, LayerMask layer, string tag, out RaycastHit hit, bool debug = false)
        {
            Ray ray = new Ray(from, (to - from).normalized);
            bool result = false;
            hit = new RaycastHit();
            RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(from, to), layer);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                {
                    result = true;
                    hit = hits[i];
                }
            }
            if (debug)
            {
                Debug.DrawLine(from, to, Color.grey);
                EditorTools.DrawLineInEditor(from, to, Color.grey);
                EditorTools.DrawSphereInEditor(result ? from + (ray.direction * hit.distance) : to, 0.05f, Color.grey);
            }
            return result;
        }
        public static bool CastToSphereOnly(this Vector3 from, Vector3 to, LayerMask layer, string tag, float radius, bool debug = false)
        {
            Ray ray = new Ray(from, (to - from).normalized);
            bool result = false;
            RaycastHit hit = new RaycastHit();
            var hits = Physics.SphereCastAll(ray, radius, Vector3.Distance(from, to), layer);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                    {
                        result = true;
                        hit = hits[i];
                        hit.distance += radius;
                    }
                }
            }
            if (debug)
            {
                Debug.DrawLine(from, to, Color.grey);
                EditorTools.DrawLineInEditor(from, to, Color.grey);
                EditorTools.DrawSphereInEditor(result ? from + (ray.direction * hit.distance) : to, 0.05f, Color.grey);
            }
            return result;
        }

        public static bool CastToSphereOnly(this Vector3 from, Vector3 to, LayerMask layer, string tag, float radius, out RaycastHit hit, bool debug = false)
        {
            Ray ray = new Ray(from, (to - from).normalized);
            bool result = false;
            hit = new RaycastHit();
            var hits = Physics.SphereCastAll(ray, radius, Vector3.Distance(from, to), layer);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                    {
                        result = true;
                        hit = hits[i];
                        hit.distance += radius;
                    }
                }
            }
            if (debug)
            {
                Debug.DrawLine(from, to, Color.grey);
                EditorTools.DrawLineInEditor(from, to, Color.grey);
                EditorTools.DrawSphereInEditor(result ? from + (ray.direction * hit.distance) : to, 0.05f, Color.grey);
            }
            return result;
        }

        public static bool MultilayerCast(this Vector3 from, Vector3 to, LayerMask layer, string tag, float[] radius, out RaycastHit hit, out int ID)
        {
            Ray ray = new Ray(from, (to - from).normalized);
            bool result = false;
            hit = new RaycastHit();
            ID = 0;
            for (int i = 0; i < radius.Length; i++)
            {
                var hits = Physics.SphereCastAll(ray, radius[i], Vector3.Distance(from, to), layer);
                if (hits.Length > 0)
                {
                    for (int t = 0; t < hits.Length; t++)
                    {
                        if (hits[t].distance > 0 && hits[t].collider.gameObject.CompareTag(tag) && (hits[t].distance < hit.distance || hit.distance == 0))
                        {
                            result = true;
                            hit = hits[t];
                            hit.distance += radius[i];
                            ID = i;
                        }
                    }
                }
                if (!result)
                {
                    hits = Physics.RaycastAll(ray, Vector3.Distance(from, to), layer);
                    for (int t = 0; t < hits.Length; t++)
                    {
                        if (hits[t].distance > 0 && hits[t].collider.gameObject.CompareTag(tag) && (hits[t].distance < hit.distance || hit.distance == 0))
                        {
                            result = true;
                            hit = hits[t];
                            ID = i;
                        }
                    }
                }
            }
            return result;
        }

        #endregion Casts
        #region Texture Additions
        public static Texture2D Update(this Texture2D texture, Vector2Int pos, Vector2Int size, Color32[] pixels, bool drawOver = true, bool apply = false)
        {
            if (!drawOver)
            {
                pixels = pixels.BlendWith(texture.GetPixels(pos.x, pos.y, size.x, size.y).Color32());
            }
            texture.SetPixels32(pos.x, pos.y, size.x, size.y, pixels);
            if (apply)
            {
                texture.Apply(false);
            }
            return texture;
        }
        #endregion Texture Additions
        #region Color Additions
        public static Color[] SetColor<Color>(this Color[] pixels, Color color)
        {
            for (int i = 0; i < pixels.Length; ++i)
                pixels[i] = color;
            return pixels;
        }

        public static Color32[] Color32(this Color[] colors)
        {
            Color32[] r = new Color32[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                r[i] = colors[i];
            }
            return r;
        }

        public static Color32[] BlendWith(this Color32[] colors, Color32[] pixels, double amount = 0.5f)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = colors[i].BlendWith(pixels[i], amount);
            }
            return pixels;
        }

        public static Color32 BlendWith(this Color32 color, Color32 backColor, double amount)
        {
            byte r = (byte)((color.r * amount) + (backColor.r * (1 - amount)));
            byte g = (byte)((color.g * amount) + (backColor.g * (1 - amount)));
            byte b = (byte)((color.b * amount) + (backColor.b * (1 - amount)));
            byte a = (byte)((color.a * amount) + (backColor.a * (1 - amount)));
            return new Color32(r, g, b, a);
        }
        public static Vector4 ToVector4(this Color32 color)
        {
            Color t = color;
            return new Vector4(t.r, t.g, t.b, t.a);
        }
        public static Vector4 ToVector4(this Color color)
        {
            return new Vector4(color.r, color.g, color.b, color.a);
        }
        public static float[] ToArray(this Color color)
        {
            return new float[] { color.r, color.g, color.b, color.a };
        }

        public static Color SetR(this Color color, float r)
        {
            return new Color(r, color.g, color.b, color.a);
        }
        public static Color SetG(this Color color, float g)
        {
            return new Color(color.r, g, color.b, color.a);
        }
        public static Color SetB(this Color color, float b)
        {
            return new Color(color.r, color.g, b, color.a);
        }
        public static Color SetA(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }
        public static Color SetRGB(this Color color, float r, float g, float b)
        {
            return new Color(r, g, b, color.a);
        }
        public static Color Set(this Color _, float r, float g, float b, float a)
        {
            return new Color(r, g, b, a);
        }

        #endregion Color Additions
        #region Vector3 Additions
        #region Geometry
        public static Vector3 GetCorner(this Vector3 pos, Vector3 corner, Vector3 scale)
        {
            return pos + corner.MultiplyEach(scale / 2);
        }
        #endregion Geometry
        #region Operations
        public static Vector3 Add(this Vector3 vector, float value)
        {
            return new Vector3(vector.x + value, vector.y + value, vector.z + value);
        }
        public static Vector3 Substract(this Vector3 vector, float value)
        {
            return new Vector3(vector.x - value, vector.y - value, vector.z - value);
        }
        public static Vector3 Multiply(this Vector3 vector, float value)
        {
            return new Vector3(vector.x * value, vector.y * value, vector.z * value);
        }
        public static Vector3 Divide(this Vector3 vector, float value)
        {
            return new Vector3(vector.x / value, vector.y / value, vector.z / value);
        }
        public static Vector3 MultiplyEach(this Vector3 v, Vector3 vector)
        {
            return new Vector3(v.x * vector.x, v.y * vector.y, v.z * vector.z);
        }

        public static Vector3 DivideEach(this Vector3 vector, Vector3 value)
        {
            return new Vector3(vector.x / value.x, vector.y / value.y, vector.z / value.z);
        }
        public static Vector3 Multiply(this Vector3 vector, Vector3 value)
        {
            return new Vector3(vector.x * value.x, vector.y * value.y, vector.z * value.z);
        }
        public static Vector3 Divide(this Vector3 vector, Vector3 value)
        {
            return new Vector3(vector.x / value.x, vector.y / value.y, vector.z / value.z);
        }

        public static Vector3 Inverse(this Vector3 vector)
        {
            return new Vector3(vector.x == 0?vector.x:1/vector.x, vector.y == 0 ? vector.y : 1 / vector.y, vector.z == 0 ? vector.z : 1 / vector.z);
        }
        #endregion Operations
        #region Validation
        public static bool IsZero(this Vector3 vector)
        {
            return vector.Equals(Vector3.zero);
        }
        public static bool IsInfinite(this Vector3 vector)
        {
            return vector.Equals(Vector3.positiveInfinity) || vector.Equals(Vector3.negativeInfinity);
        }
        public static float IfZero(this float f, float value)
        {
            return f.Equals(0) ? value : f;
        }
        public static Vector3 IfZero(this Vector3 vector, Vector3 value)
        {
            return vector.SetX(vector.x.IfZero(value.x)).SetY(vector.y.IfZero(value.y)).SetZ(vector.z.IfZero(value.z));
        }
        public static Vector3 IfZero(this Vector3 vector, float value)
        {
            return vector.SetX(vector.x.IfZero(value)).SetY(vector.y.IfZero(value)).SetZ(vector.z.IfZero(value));
        }
        public static float IfNZero(this float f, float value, bool keepSign = false)
        {
            return f.Equals(0) ? f : value * Mathf.Sign(f);
        }
        public static Vector3 IfNZero(this Vector3 vector, Vector3 value, bool keepSign = false)
        {
            return vector.SetX(vector.x.IfNZero(value.x, keepSign)).SetY(vector.y.IfNZero(value.y, keepSign)).SetZ(vector.z.IfNZero(value.z, keepSign));
        }
        public static Vector3 IfNZero(this Vector3 vector, float value, bool keepSign = false)
        {
            return vector.SetX(vector.x.IfNZero(value, keepSign)).SetY(vector.y.IfNZero(value, keepSign)).SetZ(vector.z.IfNZero(value, keepSign));
        }
        #endregion Validation
        #region Direction
        public static Vector3 Remap(this Vector3 vector, Vector3 direction)
        {
            return (direction.Right() * vector.x)
                   + (direction.Up() * vector.y)
                   + (direction.Forward() * vector.z);
        }
        public static Vector3 Rotate(this Vector3 vector, float angle, Vector3 axis) // https://answers.unity.com/questions/46770/rotate-a-vector3-direction.html
        {
            return Quaternion.AngleAxis(angle, axis) * vector;
        }
        public static Vector3 RotateAlong(this Vector3 vector, Vector3 referenceVector)
        {
            return Quaternion.Euler(referenceVector) * vector;
        }
        public static Vector3 Right(this Vector3 vector)
        {
            return -Vector3.Cross(vector.normalized, Vector3.up) * vector.magnitude;
        }
        public static Vector3 Left(this Vector3 vector)
        {
            return Vector3.Cross(vector.normalized, Vector3.up) * vector.magnitude;
        }
        public static Vector3 Up(this Vector3 vector)
        {
            return Vector3.Cross(Vector3.Cross(vector.normalized, Vector3.up), vector.normalized) * vector.magnitude;
        }
        public static Vector3 Down(this Vector3 vector)
        {
            return -Vector3.Cross(Vector3.Cross(vector.normalized, Vector3.up), vector.normalized) * vector.magnitude;
        }
        public static Vector3 Forward(this Vector3 vector)
        {
            return vector;
        }
        public static Vector3 Back(this Vector3 vector)
        {
            return -vector;
        }
        public static Vector3 Direction(this Vector3 from, Vector3 to)
        {
            return (to - from).normalized;
        }
        public static Vector3 Direction(this Vector3 from, Vector3 to, bool inverse)
        {
            return inverse ? (from - to).normalized : (to - from).normalized;
        }
        public static Quaternion FullDirection(this Vector3 from, Vector3 to)
        {
            Quaternion q;
            var a = Vector3.Dot(from, to);
            q.x = q.y = q.z = a;
            q.w = Mathf.Sqrt(from.sqrMagnitude * to.sqrMagnitude) + Vector3.Dot(from, to);
            return q.normalized;
        }
        public static Quaternion FullToQuaternion(this Vector3 vector)
        {
            var angle = Math.Atan2(vector.z, vector.x); // Note: I expected atan2(z,x) but OP reported success with atan2(x,z) instead! Switch around if you see 90° off.
            return new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
        }
        public static Quaternion ToQuaternion(this Vector3 euler)
        {
            return Quaternion.Euler(euler);
        }
        public static Quaternion DirectionToQuaternion(this Vector3 direction)
        {
            return Quaternion.LookRotation(direction, Vector3.up);
        }
        public static Quaternion ToQuaternion(this Vector3 NormalizedVector, Quaternion rotation)
        {
            return Quaternion.FromToRotation(Vector3.up, NormalizedVector) * rotation;
        }
        public static Vector3 Reflect(this Vector3 vector, Vector3 normal)
        {
            return vector - 2 * Vector3.Dot(vector, normal) * normal;
        }
        public static Vector3 Movealong(this Vector3 vector, Vector3 normal)
        {
            return vector - Vector3.Dot(vector, normal) * normal;
        }
        public static Vector2 Right(this Vector2 vector)
        {
            return Vector2.Perpendicular(Vector2.Perpendicular(Vector2.Perpendicular(vector)));
        }
        public static Vector2 Left(this Vector2 vector)
        {
            return Vector2.Perpendicular(vector);
        }
        public static Vector2 Up(this Vector2 vector)
        {
            return vector;
        }
        public static Vector2 Down(this Vector2 vector)
        {
            return Vector2.Perpendicular(Vector2.Perpendicular(vector));
        }
        #endregion Direction
        #region Min Max
        public static Vector3 Min(this Vector3 vector, float min)
        {
            return new Vector3(vector.x.Min(min), vector.y.Min(min), vector.z.Min(min));
        }
        public static Vector3 Max(this Vector3 vector, float max)
        {
            return new Vector3(vector.x.Max(max), vector.y.Max(max), vector.z.Max(max));
        }
        public static Vector3 Max(this Vector3 vector, float max, bool ignoreSign)
        {
            if (!ignoreSign)
                return vector.Max(max);
            return new Vector3(vector.x.Abs() > max.Abs() ? vector.x : max, vector.y.Abs() > max.Abs() ? vector.y : max, vector.z.Abs() > max.Abs() ? vector.z : max);
        }
        public static Vector3 MaxX(this Vector3 vector, float max)
        {
            return new Vector3(vector.x.Max(max), vector.y, vector.z);
        }
        public static Vector3 MaxY(this Vector3 vector, float max)
        {
            return new Vector3(vector.x, vector.y.Max(max), vector.z);
        }
        public static Vector3 MaxZ(this Vector3 vector, float max)
        {
            return new Vector3(vector.x, vector.y, vector.z.Max(max));
        }
        public static Vector3 MaxXZ(this Vector3 vector, float max)
        {
            return new Vector3(vector.x.Max(max), vector.y, vector.z.Max(max));
        }
        #endregion Min Max
        #region Clamp

        public static Vector3 Clamp(this Vector3 vector, float min, float max)
        {
            return new Vector3(vector.x.Clamp(min, max), vector.y.Clamp(min, max), vector.z.Clamp(min, max));
        }
        public static Vector3 Clamp01(this Vector3 vector)
        {
            return new Vector3(vector.x.Clamp01(), vector.y.Clamp01(), vector.z.Clamp01());
        }
        public static Vector3 ClampX(this Vector3 vector, float min, float max)
        {
            return new Vector3(vector.x.Clamp(min, max), vector.y, vector.z);
        }
        public static Vector3 ClampY(this Vector3 vector, float min, float max)
        {
            return new Vector3(vector.x, vector.y.Clamp(min, max), vector.z);
        }
        public static Vector3 ClampZ(this Vector3 vector, float min, float max)
        {
            return new Vector3(vector.x, vector.y, vector.z.Clamp(min, max));
        }
        public static Vector3 ClampXZ(this Vector3 vector, float min, float max)
        {
            return new Vector3(vector.x.Clamp(min, max), vector.y, vector.z.Clamp(min, max));
        }
        public static Vector3 ClampXZTotal(this Vector3 vector, float max)
        {
            Vector2 r = new Vector2();
            r += new Vector2(vector.x.Abs(), vector.z.Abs()).normalized * max;
            return new Vector3(vector.x.Clamp(-r.x, r.x), vector.y, vector.z.Clamp(-r.y, r.y));
        }
        public static Vector3 ClampXZKeepRelation(this Vector3 vector, float min, float max)
        {
            if (vector.x != 0 && vector.z != 0)
            {
                if (vector.x > max || vector.z > max)
                {
                    if (vector.x > vector.z)
                    {
                        float coeff = vector.x / vector.z;
                        vector.x = vector.x.Clamp(min, max);
                        vector.z = vector.x / coeff;
                    }
                    else
                    {
                        float coeff = vector.z / vector.x;
                        vector.z = vector.z.Clamp(min, max);
                        vector.x = vector.z / coeff;
                    }
                }
                if (vector.x < min || vector.z < min)
                {
                    if (vector.x < vector.z)
                    {
                        float coeff = vector.x / vector.z;
                        vector.x = vector.x.Clamp(min, max);
                        vector.z = vector.x / coeff;
                    }
                    else
                    {
                        float coeff = vector.z / vector.x;
                        vector.z = vector.z.Clamp(min, max);
                        vector.x = vector.z / coeff;
                    }
                }
            }
            return new Vector3(vector.x.Clamp(min, max), vector.y, vector.z.Clamp(min, max));
        }
        #endregion Clamp
        #region Add
        public static Vector3 Sum(this Vector3[] vectors)
        {
            Vector3 result = new Vector3 { };
            for (int i = 0; i < vectors.Length; i++)
            {
                result += vectors[i];
            }
            return result;
        }
        #endregion Add
        #region isBetween
        public static bool IsBetween(this Vector3 vector, Vector3 bound1, Vector3 bound2)
        {
            return vector.x.IsBetween(bound1.x, bound2.x) && vector.y.IsBetween(bound1.y, bound2.y) && vector.z.IsBetween(bound1.z, bound2.z);
        }
        #endregion isBetween
        #region Set Partial
        public static Vector3 SetX(this Vector3 vector, float x)
        {
            vector.x = x;
            return vector;
        }
        public static Vector3 SetY(this Vector3 vector, float y)
        {
            vector.y = y;
            return vector;
        }
        public static Vector3 SetZ(this Vector3 vector, float z)
        {
            vector.z = z;
            return vector;
        }
        public static Vector3 SetX(this Vector3 vector, double x)
        {
            vector.x = (float)x;
            return vector;
        }
        public static Vector3 SetY(this Vector3 vector, double y)
        {
            vector.y = (float)y;
            return vector;
        }
        public static Vector3 SetZ(this Vector3 vector, double z)
        {
            vector.z = (float)z;
            return vector;
        }
        #endregion Set Partial
        #region ToVector2
        public static Vector2 ToVector2(this Vector3 vector)
        {
            return vector;
        }
        #endregion ToVector2
        #region Random
        public static Vector3 Randomize(this Vector3 _)
        {
            return UnityEngine.Random.insideUnitSphere.normalized;
        }
        public static Vector3 Randomize(this Vector3 vector, bool x)
        {
            return new Vector3(x ? UnityEngine.Random.value : vector.x, UnityEngine.Random.value, UnityEngine.Random.value);
        }
        public static Vector3 Randomize(this Vector3 vector, bool x, bool y)
        {
            return new Vector3(x ? UnityEngine.Random.value : vector.x, y ? UnityEngine.Random.value : vector.y);
        }
        public static Vector3 Randomize(this Vector3 vector, bool x, bool y, bool z)
        {
            return new Vector3(x ? UnityEngine.Random.value : vector.x, y ? UnityEngine.Random.value : vector.y, z ? UnityEngine.Random.value : vector.z);
        }
        public static Vector3 Randomize(this Vector3 vector, float maxPercDiff)
        {
            Vector3 r = UnityEngine.Random.insideUnitSphere.normalized;
            float diff = Vector3.Angle(vector, r) / 180; // perc diff
            float res = Mathf.Max(0, diff - maxPercDiff); // perc diff that matters
            res = 1 - res; // perc to apply to new to get closer to old
            return Vector3.Lerp(r, vector, res).normalized; // a + (b -a ) * x
        }
        public static Vector3 RandomizeInBounds(this Vector3 vector, float maxPercDiff, Vector3 position, Bounds bounds)
        {
            Vector3 r = position.Direction(GameTools.RandomPointInBounds(bounds));
            float diff = Vector3.Angle(vector, r) / 180; // perc diff
            float res = Mathf.Max(0, diff - maxPercDiff); // perc diff that matters
            return Vector3.Lerp(r, vector, res).normalized; // a + (b -a ) * x
        }
        #endregion Random
        #region NormalizeTo
        public static Vector3 NormalizeTo(this Vector3 vector, float n)
        {
            return vector.normalized * n;
        }

        public static Vector3 NormalizeTo(this Vector3 vector, Vector3 n)
        {
            return vector.normalized.Multiply(n);
        }
        #endregion NormalizeTo
        #endregion Vector3 Additions
        #region Vector2 Additions
        #region Value
        public static Vector2 Abs(this Vector2 vector)
        {
            return new Vector2(vector.x.Abs(), vector.y.Abs());
        }
        #endregion Value
        #region Geometry
        public static Vector2 PointOnSlope(this Vector2 point, float distanceX, float angle)
        {
            var r = angle.RadianToVector2();
            return point + new Vector2(0, r.y.Abs());
        }
        public static Vector2 PointAtAngle(this Vector2 point, float distanceHypotenuse, float angle)
        {
            return point + (angle.DegreeToVector2() * distanceHypotenuse);
        }
        public static Vector2 DegreeToVector2(this float degree)
        {
            return (degree * Mathf.Deg2Rad).RadianToVector2();
        }
        public static Vector2 RadianToVector2(this float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
        #endregion Geometry
        #region Direction
        public static Vector2 Reflect(this Vector2 vector, Vector2 normal)
        {
            return vector - 2 * Vector2.Dot(vector, normal) * normal;
        }
        public static Vector2 Movealong(this Vector2 vector, Vector2 normal)
        {
            return vector - Vector2.Dot(vector, normal) * normal;
        }
        #endregion Direction
        #region isBetween
        public static bool IsBetween(this Vector2 vector, Vector2 bound1, Vector2 bound2)
        {
            return vector.x.IsBetween(bound1.x, bound2.x) && vector.y.IsBetween(bound1.y, bound2.y);
        }
        #endregion isBetween
        #region Set Partial
        public static Vector2 SetX(this Vector2 vector, float x)
        {
            vector.x = x;
            return vector;
        }
        public static Vector2 SetY(this Vector2 vector, float y)
        {
            vector.y = y;
            return vector;
        }
        public static Vector2 SetX(this Vector2 vector, double x)
        {
            vector.x = (float)x;
            return vector;
        }
        public static Vector2 SetY(this Vector2 vector, double y)
        {
            vector.y = (float)y;
            return vector;
        }
        #endregion Set Partial
        #region ToVector3
        public static Vector3 ToVector3(this Vector2 vector)
        {
            return vector;
        }
        public static Vector3 ToVector3(this Vector2 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }
        #endregion ToVector3
        #endregion Vector2 Additions
        #region float Additions
        #region RemoveInfinity
        public static float RemoveInfinity(this float value, bool InfiniteToMin = false)
        {
            if (value == float.PositiveInfinity)
                return InfiniteToMin ? float.MinValue : float.MaxValue;
            if (value == float.NegativeInfinity)
                return InfiniteToMin ? float.MaxValue : float.MinValue;
            return value;
        }
        #endregion RemoveInfinity
        #region GetMaxs
        public static float GetMax(this Vector3 vector, bool abs = false)
        {
            return vector.x.Max(abs, vector.y, vector.z);
        }

        public static float GetMaxXY(this Vector3 vector, bool abs = false)
        {
            return vector.x.Max(abs, vector.y);
        }
        public static float GetMaxXZ(this Vector3 vector, bool abs = false)
        {
            return vector.x.Max(abs, vector.z);
        }
        public static float GetMaxYZ(this Vector3 vector, bool abs = false)
        {
            return vector.y.Max(abs, vector.z);
        }
        #endregion GetMaxs
        #region Min Max
        public static float Min(this float value, params float[] values)
        {
            return Mathf.Min(value, values.Min());
        }
        public static float Max(this float value, params float[] values)
        {
            return Mathf.Max(value, values.Max());
        }
        public static float Min(this float value, bool abs = false, params float[] values)
        {
            return abs ? Mathf.Min(value.Abs(), values.Abs().Min()) : Mathf.Min(value, values.Min());
        }
        public static float Max(this float value, bool abs = false, params float[] values)
        {
            return abs ? Mathf.Max(value.Abs(), values.Abs().Max()) : Mathf.Max(value, values.Max());
        }
        public static float Min(this float value, bool abs = false, bool ignoreInfinity = false, params float[] values)
        {
            if (ignoreInfinity)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = values[i].RemoveInfinity(true);
                value = value.RemoveInfinity();
            }
            return abs ? Mathf.Min(value.Abs(), values.Abs().Min()) : Mathf.Min(value, values.Min());
        }
        public static float Max(this float value, bool abs = false, bool ignoreInfinity = false, params float[] values)
        {
            if (ignoreInfinity)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = values[i].RemoveInfinity(true);
                value = value.RemoveInfinity();
            }
            return abs ? Mathf.Max(value.Abs(), values.Abs().Max()) : Mathf.Max(value, values.Max());
        }
        #endregion Min Max
        #region Clamp
        public static float Clamp(this float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }
        public static double Clamp(this double value, double min, double max)
        {
            return Math.Max(Math.Min(value, max), min);
        }
        public static float Clamp(this float value, double min, double max)
        {
            return Mathf.Clamp(value, (float)max, (float)min);
        }
        public static float Clamp01(this float value)
        {
            return Mathf.Clamp01(value);
        }
        #endregion Clamp
        #region Abs, Sum, Minus
        public static float Abs(this float f)
        {
            return Mathf.Abs(f);
        }
        public static float[] Abs(this float[] f)
        {
            for (int i = 0; i < f.Length; i++)
                f[i] = f[i].Abs();
            return f;
        }
        public static float Sum(this Vector2 vector)
        {
            return vector.x + vector.y;
        }
        public static float Sum(this Vector3 vector)
        {
            return vector.x + vector.y + vector.z;
        }
        public static float Minus(this float f, float value, bool abs = false)
        {
            return abs ? Mathf.Abs(f - value) : f - value;
        }
        public static float Minus(this float f, float value, bool maxFirst = false, bool abs = false)
        {
            return abs ? (Mathf.Max(f, value) - Mathf.Min(f, value)).Abs() : Mathf.Max(f, value) - Mathf.Min(f, value);
        }
        public static float MinusAngle(this float f, float value)
        {
            float r = f - value;
            return r + ((r > 180) ? -360 : (r < -180) ? 360 : 0);
        }
        public static float MinusAngle(this float f, float value, bool maxFirst = false)
        {
            float r = Mathf.Max(f, value) - Mathf.Min(f, value);
            return r + ((r > 180) ? -360 : (r < -180) ? 360 : 0);
        }
        #endregion Abs, Sum
        #region IsBetween
        public static bool IsBetween(this float value, float first, float second)
        {
            return value >= Mathf.Min(first, second) && value <= Mathf.Max(first, second);
        }
        #endregion IsBetween
        #endregion float Additions
        #region Basic conversions
        public static bool ToBool(this int value)
        {
            return value <= 0;
        }
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }
        #endregion Basic conversions
        #region Transform Additions
        #region Anchors
        public static Vector3 GetWorld(this Transform transform, Vector3 localSpace)
        {
            return transform.TransformVector(localSpace);
        }
        public static Vector3 GetWorldPosition(this Transform transform, Vector3 localSpace)
        {
            return transform.position + transform.TransformVector(localSpace);
        }
        public static Vector3 GetLocal(this Transform transform, Vector3 worldSpace)
        {
            return transform.InverseTransformVector(worldSpace);
        }
        public static Vector3 GetLocalPosition(this Transform transform, Vector3 worldSpace)
        {
            return transform.InverseTransformPoint(transform.position + worldSpace);
        }
        #endregion Anchors
        public static int ActiveChildCount(this Transform transform)
        {
            int r = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    r++;
                }
            }
            return r;
        }
        public static int ChildsOfTypeCount(this Transform transform, Type type)
        {
            int r = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetType() == type)
                {
                    r++;
                }
            }
            return r;
        }
        public static int ChildsWithComponent(this Transform transform, Type type)
        {
            int r = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(type, out _))
                {
                    r++;
                }
            }
            return r;
        }
        public static int ActiveChildsOfTypeCount(this Transform transform, Type type)
        {
            int r = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetType() == type && transform.GetChild(i).gameObject.activeSelf)
                {
                    r++;
                }
            }
            return r;
        }
        public static int ActiveChildsWithComponent(this Transform transform, Type type)
        {
            int r = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(type, out _) && transform.GetChild(i).gameObject.activeSelf)
                {
                    r++;
                }
            }
            return r;
        }
        public static Transform FindDeep(this Transform transform, string name)
        {
            Transform[] res = transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform r in res)
            {
                if (r.name == name)
                {
                    return r;
                }
            }
            return null;
        }
        public static GameObject FindDeep(this GameObject gameObject, string name)
        {
            var r = gameObject.transform.FindDeep(name);
            return r == null ? null : r.gameObject;
        }
        public static Transform GetParent(this Transform transform, int num)
        {
            if (num == 0)
                return transform;
            return transform.parent.GetParent(num - 1);
        }
        public static Transform FindParentDeep(this Transform transform, string name)
        {
            if (transform == null)
                return null;
            Transform res = transform.parent;
            if (res.name == name)
            {
                return res;
            }
            return FindParentDeep(res, name);
        }
        public static GameObject FindParentDeep(this GameObject gameObject, string name)
        {
            var r = gameObject.transform.FindParentDeep(name);
            return r == null ? null : r.gameObject;
        }
        public static Transform[] GetChildsWithComponent(this Transform transform, Type type)
        {
            Transform[] r = new Transform[] { };
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(type, out _))
                {
                    r.Add(transform.GetChild(i));
                }
            }
            return r;
        }
        public static Transform FindParentWithComponent(this Transform transform, Type type)
        {
            if (transform == null)
                return null;
            Transform res = transform.parent;
            if (res.TryGetComponent(type, out _))
            {
                return res;
            }
            return FindParentWithComponent(res, type);
        }
        public static GameObject FindParentWithComponent(this GameObject gameObject, Type type)
        {
            var r = gameObject.transform.FindParentWithComponent(type);
            return r == null ? null : r.gameObject;
        }
        public static Rect GetWorldRect(this RectTransform rt, Vector2? scale = null)
        {
            if (!scale.HasValue)
                scale = Vector2.one;
            // Convert the rectangle to world corners and grab the top left
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            Vector3 topLeft = corners[0];

            // Rescale the size appropriately based on the current Canvas scale
            Vector2 scaledSize = new Vector2(scale.Value.x * rt.rect.size.x, scale.Value.y * rt.rect.size.y);

            return new Rect(topLeft, scaledSize);
        }
        public static Vector3 AnchorNeutralPosition(this RectTransform rectTransform)
        {
            return rectTransform.position + (rectTransform.rect.center * rectTransform.lossyScale).ToVector3();
        }
        public static Vector3 AnchorNeutralPosition(this RectTransform rectTransform, bool offset)
        {
            return rectTransform.position + ((rectTransform.rect.center - (offset ? rectTransform.anchoredPosition : Vector2.zero)) * rectTransform.lossyScale).ToVector3();
        }
        #endregion Transform Additions
        #region Distances
        public static float Distance(this Transform transform, Transform compared) // transform transform
        {
            return Vector3.Distance(transform.position, compared.position);
        }
        public static float Distance(this Transform transform, GameObject compared) // transform gameobject
        {
            return Vector3.Distance(transform.position, compared.transform.position);
        }
        public static float Distance(this Transform transform, Vector3 compared) // transform vector3
        {
            return Vector3.Distance(transform.position, compared);
        }
        public static float Distance(this GameObject transform, Transform compared) // gameobject transform
        {
            return Vector3.Distance(transform.transform.position, compared.position);
        }
        public static float Distance(this GameObject transform, GameObject compared) // gameobject gameobject
        {
            return Vector3.Distance(transform.transform.position, compared.transform.position);
        }
        public static float Distance(this GameObject transform, Vector3 compared) // gameobject vector3
        {
            return Vector3.Distance(transform.transform.position, compared);
        }
        public static float Distance(this Vector3 transform, Transform compared) // vector3 transform
        {
            return Vector3.Distance(transform, compared.position);
        }
        public static float Distance(this Vector3 transform, GameObject compared) // vector3 gameobject
        {
            return Vector3.Distance(transform, compared.transform.position);
        }
        public static float Distance(this Vector3 transform, Vector3 compared) // vector3 vector3
        {
            return Vector3.Distance(transform, compared);
        }
        public static Transform GetClosest(this Transform[] transforms, Transform referenceFrame)
        {
            Transform closest = transforms[0];
            float t = referenceFrame.Distance(transforms[0]);
            for (int i = 1; i < transforms.Length; i++)
            {
                if (referenceFrame.Distance(transforms[i]) < t)
                {
                    t = referenceFrame.Distance(transforms[i]);
                    closest = transforms[i];
                }
            }
            return closest;
        }
        public static Transform GetClosest(this Transform[] transforms, Vector3 referenceFrame)
        {
            Transform closest = transforms[0];
            float t = referenceFrame.Distance(transforms[0]);
            for (int i = 1; i < transforms.Length; i++)
            {
                if (referenceFrame.Distance(transforms[i]) < t)
                {
                    t = referenceFrame.Distance(transforms[i]);
                    closest = transforms[i];
                }
            }
            return closest;
        }
        public static Transform GetClosest(this Transform[] transforms, GameObject referenceFrame)
        {
            Transform closest = transforms[0];
            float t = referenceFrame.Distance(transforms[0]);
            for (int i = 1; i < transforms.Length; i++)
            {
                if (referenceFrame.Distance(transforms[i]) < t)
                {
                    t = referenceFrame.Distance(transforms[i]);
                    closest = transforms[i];
                }
            }
            return closest;
        }
        public static GameObject GetClosest(this GameObject[] gameObjects, Transform referenceFrame)
        {
            GameObject closest = gameObjects[0];
            float t = referenceFrame.Distance(gameObjects[0]);
            for (int i = 1; i < gameObjects.Length; i++)
            {
                if (referenceFrame.Distance(gameObjects[i]) < t)
                {
                    t = referenceFrame.Distance(gameObjects[i]);
                    closest = gameObjects[i];
                }
            }
            return closest;
        }
        public static GameObject GetClosest(this GameObject[] gameObjects, Vector3 referenceFrame)
        {
            GameObject closest = gameObjects[0];
            float t = referenceFrame.Distance(gameObjects[0]);
            for (int i = 1; i < gameObjects.Length; i++)
            {
                if (referenceFrame.Distance(gameObjects[i]) < t)
                {
                    t = referenceFrame.Distance(gameObjects[i]);
                    closest = gameObjects[i];
                }
            }
            return closest;
        }
        public static GameObject GetClosest(this GameObject[] gameObjects, GameObject referenceFrame)
        {
            GameObject closest = gameObjects[0];
            float t = referenceFrame.Distance(gameObjects[0]);
            for (int i = 1; i < gameObjects.Length; i++)
            {
                if (referenceFrame.Distance(gameObjects[i]) < t)
                {
                    t = referenceFrame.Distance(gameObjects[i]);
                    closest = gameObjects[i];
                }
            }
            return closest;
        }
        public static Vector3 GetClosest(this Vector3[] gameObjects, Transform referenceFrame)
        {
            Vector3 closest = gameObjects[0];
            float t = referenceFrame.Distance(gameObjects[0]);
            for (int i = 1; i < gameObjects.Length; i++)
            {
                if (referenceFrame.Distance(gameObjects[i]) < t)
                {
                    t = referenceFrame.Distance(gameObjects[i]);
                    closest = gameObjects[i];
                }
            }
            return closest;
        }
        public static Vector3 GetClosest(this Vector3[] gameObjects, Vector3 referenceFrame)
        {
            Vector3 closest = gameObjects[0];
            float t = referenceFrame.Distance(gameObjects[0]);
            for (int i = 1; i < gameObjects.Length; i++)
            {
                if (referenceFrame.Distance(gameObjects[i]) < t)
                {
                    t = referenceFrame.Distance(gameObjects[i]);
                    closest = gameObjects[i];
                }
            }
            return closest;
        }
        public static Vector3 GetClosest(this Vector3[] gameObjects, GameObject referenceFrame)
        {
            Vector3 closest = gameObjects[0];
            float t = referenceFrame.Distance(gameObjects[0]);
            for (int i = 1; i < gameObjects.Length; i++)
            {
                if (referenceFrame.Distance(gameObjects[i]) < t)
                {
                    t = referenceFrame.Distance(gameObjects[i]);
                    closest = gameObjects[i];
                }
            }
            return closest;
        }

        #endregion Distances
        #region QuaternionAdditions
        public static bool IsApproximatelyEqual(this Quaternion quat, Quaternion quaternion, float range = 0.01f)
        {
            return 1 - Mathf.Abs(Quaternion.Dot(quat, quaternion)) < range;
        }
        public static Quaternion SetX(this Quaternion quaternion, float x)
        {
            quaternion.x = x;
            return quaternion;
        }
        public static Quaternion SetY(this Quaternion quaternion, float y)
        {
            quaternion.y = y;
            return quaternion;
        }
        public static Quaternion SetZ(this Quaternion quaternion, float z)
        {
            quaternion.z = z;
            return quaternion;
        }
        public static Quaternion SetW(this Quaternion quaternion, float w)
        {
            quaternion.w = w;
            return quaternion;
        }
        #endregion QuaternionAdditions
    }
    #endregion Extensions
    namespace Utilities
    {
        #region Utilities
        public class GameTools
        {
            #region Vector3 Additions
            public static Vector3 Rotate(Vector3 vector, float angle, Vector3 axis) // https://answers.unity.com/questions/46770/rotate-a-vector3-direction.html
            {
                return Quaternion.AngleAxis(angle, axis) * vector;
            }
            public static Vector3 Sum(Vector3[] vectors)
            {
                Vector3 result = new Vector3 { };
                for (int i = 0; i < vectors.Length; i++)
                {
                    result += vectors[i];
                }
                return result;
            }
            #region Direction
            public static Vector3 DirectionTo(Vector3 from, Vector3 to)
            {
                return (to - from).normalized;
            }
            public static Vector3 Direction(Vector3 from, Vector3 to, bool inverse)
            {
                return inverse ? (from - to).normalized : (to - from).normalized;
            }
            public static Vector2 DirectionTo(Vector2 from, Vector2 to)
            {
                return (to - from).normalized;
            }
            public static Vector2 Direction(Vector2 from, Vector2 to, bool inverse)
            {
                return inverse ? (from - to).normalized : (to - from).normalized;
            }
            public static Vector3 Reflect(Vector3 vector, Vector3 normal)
            {
                return vector - 2 * Vector3.Dot(vector, normal) * normal;
            }
            public static Vector3 Movealong(Vector3 vector, Vector3 normal)
            {
                return vector - Vector3.Dot(vector, normal) * normal;
            }
            #endregion Direction
            #region Random
            public static Vector3 RandomVector()
            {
                return Random.insideUnitCircle.normalized;
            }
            public static Vector3 RandomVector(Vector3 vector, float x = -1, float y = -1, float z = -1)
            {
                return new Vector3(x == -1 ? Random.value : vector.x, y == -1 ? Random.value : vector.y, z == -1 ? Random.value : vector.z);
            }
            public static Vector3 RandomVector(Vector3 vector, float maxPercDiff)
            {
                Vector3 r = Random.insideUnitSphere.normalized;
                float diff = Vector3.Angle(vector, r) / 180; // perc diff
                float res = Mathf.Max(0, diff - maxPercDiff); // perc diff that matters
                res = 1 - res; // perc to apply to new to get closer to old
                return Vector3.Lerp(r, vector, res).normalized; // a + (b -a ) * x
            }
            public static Vector3 RandomVectorInBounds(Vector3 vector, float maxPercDiff, Vector3 position, Bounds bounds)
            {
                Vector3 r = position.Direction(RandomPointInBounds(bounds));
                float diff = Vector3.Angle(vector, r) / 180; // perc diff
                float res = Mathf.Max(0, diff - maxPercDiff); // perc diff that matters
                return Vector3.Lerp(r, vector, res).normalized; // a + (b -a ) * x
            }
            public static Vector3 RandomPointInBounds(Bounds bounds)
            {
                return new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    Random.Range(bounds.min.z, bounds.max.z)
                );
            }
            #endregion Random
            #endregion Vector3 Additions
            #region Vector2 Additions
            public static Vector2 Reflect(Vector2 vector, Vector2 normal)
            {
                return vector - 2 * Vector2.Dot(vector, normal) * normal;
            }
            public static Vector2 Movealong(Vector2 vector, Vector2 normal)
            {
                return vector - Vector2.Dot(vector, normal) * normal;
            }
            #endregion Vector2 Additions
            #region Ground & Wall
            public static float FindGround(Vector3 position, float sizeCompensation, float maxDistance, LayerMask layer)
            {
                Ray groundRay = (new Ray(position, Vector3.down));
                bool ground = Physics.Raycast(groundRay, out RaycastHit groundHit, maxDistance, layer);
                if (ground)
                {
                    return groundHit.distance - sizeCompensation;
                }
                return maxDistance;
            }
            public static float FindGround(Vector3 position, float sizeCompensation, float maxDistance, LayerMask layer, out RaycastHit hit)
            {
                Ray groundRay = (new Ray(position, Vector3.down));
                bool ground = Physics.Raycast(groundRay, out RaycastHit groundHit, maxDistance, layer);
                hit = groundHit;
                if (ground)
                {
                    return groundHit.distance - sizeCompensation;
                }
                return maxDistance;
            }

            public static KeyValuePair<Vector3, bool> CheckForWall(Vector3 position, Vector3 direction, float radius, float width, string layerName = "Wall")
            {
                Ray ray = new Ray(position, direction);
                Debug.DrawRay(ray.origin, ray.direction * 5, Color.red);
                bool r = Physics.SphereCast(ray, radius, out RaycastHit hit, width / 2 - radius, LayerMask.GetMask(layerName));
                Vector3 res = Vector3.Cross(hit.normal, ray.direction);
                res = Vector3.Cross(res, hit.normal);
                return new KeyValuePair<Vector3, bool>(res, r);
            }
            #endregion
            #region Geometry
            public static bool IsInRectangle(Node node, Vector3 position)
            {
                var t1 = node.transform.position;
                return position.x.IsBetween(t1.x - (node.Dimensions.x), t1.x + (node.Dimensions.x)) && position.y.IsBetween(t1.y, t1.y + (node.Dimensions.y * 2)) && position.z.IsBetween(t1.z - (node.Dimensions.z), t1.z + (node.Dimensions.z));
            }
            public static bool IsInRectangle(TPSNode node, Vector3 position)
            {
                var t1 = node.transform.position;
                return position.x.IsBetween(t1.x - (node.Dimensions.x), t1.x + (node.Dimensions.x)) && position.y.IsBetween(t1.y, t1.y + (node.Dimensions.y * 2)) && position.z.IsBetween(t1.z - (node.Dimensions.z), t1.z + (node.Dimensions.z));
            }
            public static bool IsInRectangle(Vector3 rectPos, Vector3 rectScale, Vector3 position)
            {
                return position.x.IsBetween(rectPos.x - (rectScale.x), rectPos.x + (rectScale.x)) && position.y.IsBetween(rectPos.y, rectPos.y + (rectScale.y * 2)) && position.z.IsBetween(rectPos.z - (rectScale.z), rectPos.z + (rectScale.z));
            }

            public static Vector3 ClosestOnCube(Vector3 point, Vector3 cubePos, Vector3 cubeSize, BoxCollider box, bool debug = false)
            {
                box.transform.position = cubePos + new Vector3(0, cubeSize.y);
                box.size = cubeSize * 2;
                if (IsInRectangle(cubePos, cubeSize, point))
                {
                    return ClosestInCube(point, cubePos, cubeSize, debug);
                }
                var r = box.ClosestPointOnBounds(point);
                return r;
            }

            public static Vector3 ClosestInCube(Vector3 point, Vector3 cubePos, Vector3 cubeSize, bool debug = false)
            {
                //f1 will be x+
                //f2 will be x-
                //f3 will be y+
                //f4 will be y-
                //f5 will be z+
                //f6 will be z-
                var f1 = cubePos;
                f1.y += (cubeSize.y);
                var f2 = f1;
                var f3 = f2;
                var f4 = f3;
                var f5 = f4;
                var f6 = f5;
                f1.x += cubeSize.x;
                f2.x -= cubeSize.x;
                f3.y += cubeSize.y;
                f4.y -= cubeSize.y;
                f5.z += cubeSize.z;
                f6.z -= cubeSize.z;
                f1.y = f2.y = point.y;
                f1.z = f2.z = point.z;
                f3.z = f4.z = point.z;
                f3.x = f4.x = point.x;
                f5.x = f6.x = point.x;
                f5.y = f6.y = point.y;
                if (debug)
                {
                    EditorTools.DrawLineInEditor(f1, point, new Color(1, 0.5f, 0));
                    EditorTools.DrawLineInEditor(f2, point, new Color(1, 0.5f, 0));
                    EditorTools.DrawLineInEditor(f3, point, new Color(1, 0.5f, 0));
                    EditorTools.DrawLineInEditor(f4, point, new Color(1, 0.5f, 0));
                    EditorTools.DrawLineInEditor(f5, point, new Color(1, 0.5f, 0));
                    EditorTools.DrawLineInEditor(f6, point, new Color(1, 0.5f, 0));
                }
                return FindClosestPoint(point, new Vector3[] { f1, f2, f3, f4, f5, f6 });
            }
            public static Vector3 FindClosestPoint(Vector3 reference, Vector3[] points)
            {
                Vector3 result = points[0];
                float r = Vector3.Distance(result, reference);
                for (int i = 1; i < points.Length; i++)
                {
                    var res = Vector3.Distance(points[i], reference);
                    if (res < r)
                    {
                        result = points[i];
                        r = res;
                    }
                }
                return result;
            }

            public static Quaternion QuaternionAvoidNull(Quaternion quaternion)
            {
                if (quaternion.x == 0 && quaternion.y == 0 && quaternion.z == 0 && quaternion.w == 0)
                {
                    return Quaternion.identity;
                }
                return quaternion;
            }
            #endregion Geometry
            #region Casts
            public static bool CastTo(Vector3 from, Vector3 to, LayerMask layer, string tag, bool debug = false)
            {
                Ray ray = new Ray(from, (to - from).normalized);
                bool result = false;
                RaycastHit hit = new RaycastHit();
                var hits = Physics.SphereCastAll(ray, 0.2f, Vector3.Distance(from, to), layer);
                if (hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                        {
                            result = true;
                            hit.distance += 0.2f;
                        }
                    }
                }
                if (!result)
                {
                    hits = Physics.RaycastAll(ray, Vector3.Distance(from, to), layer);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                        {
                            result = true;
                            hit = hits[i];
                        }
                    }
                }
                if (debug)
                {
                    EditorTools.DrawLineInEditor(from, to, Color.grey);
                    EditorTools.DrawSphereInEditor(result ? from + (ray.direction * hit.distance) : to, 0.05f, Color.grey);
                }
                return result;
            }
            public static bool CastTo(Vector3 from, Vector3 to, LayerMask layer, bool debug = false)
            {
                Ray ray = new Ray(from, (to - from).normalized);
                bool result = false;
                RaycastHit hit = new RaycastHit();
                var hits = Physics.SphereCastAll(ray, 0.2f, Vector3.Distance(from, to), layer);
                if (hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].distance > 0 && (hits[i].distance < hit.distance || hit.distance == 0))
                        {
                            result = true;
                            hit.distance += 0.2f;
                        }
                    }
                }
                if (!result)
                {
                    hits = Physics.RaycastAll(ray, Vector3.Distance(from, to), layer);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].distance > 0 && (hits[i].distance < hit.distance || hit.distance == 0))
                        {
                            result = true;
                            hit = hits[i];
                        }
                    }
                }
                if (debug)
                {
                    EditorTools.DrawLineInEditor(from, to, Color.grey);
                    EditorTools.DrawSphereInEditor(result ? from + (ray.direction * hit.distance) : to, 0.05f, Color.grey);
                }
                return result;
            }
            public static bool CastTo(Vector3 from, Vector3 to, LayerMask layer, string tag, float radius, bool debug = false)
            {
                Ray ray = new Ray(from, (to - from).normalized);
                bool result = false;
                RaycastHit hit = new RaycastHit();
                var hits = Physics.SphereCastAll(ray, radius, Vector3.Distance(from, to), layer);
                if (hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                        {
                            result = true;
                            hit = hits[i];
                            hit.distance += radius;
                        }
                    }
                }
                if (!result)
                {
                    hits = Physics.RaycastAll(ray, Vector3.Distance(from, to), layer);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                        {
                            result = true;
                            hit = hits[i];
                        }
                    }
                }
                if (debug)
                {
                    EditorTools.DrawLineInEditor(from, to, Color.grey);
                    EditorTools.DrawSphereInEditor(result ? from + (ray.direction * hit.distance) : to, 0.05f, Color.grey);
                }
                return result;
            }

            public static bool CastTo(Vector3 from, Vector3 to, LayerMask layer, string tag, float radius, out RaycastHit hit, bool debug = false)
            {
                Ray ray = new Ray(from, (to - from).normalized);
                bool result = false;
                hit = new RaycastHit();
                var hits = Physics.SphereCastAll(ray, radius, Vector3.Distance(from, to), layer);
                if (hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                        {
                            result = true;
                            hit = hits[i];
                            hit.distance += radius;
                        }
                    }
                }
                if (!result)
                {
                    hits = Physics.RaycastAll(ray, Vector3.Distance(from, to), layer);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].distance > 0 && hits[i].collider.gameObject.CompareTag(tag) && (hits[i].distance < hit.distance || hit.distance == 0))
                        {
                            result = true;
                            hit = hits[i];
                        }
                    }
                }
                if (debug)
                {
                    EditorTools.DrawLineInEditor(from, to, Color.grey);
                    EditorTools.DrawSphereInEditor(result ? from + (ray.direction * hit.distance) : to, 0.05f, Color.grey);
                }
                return result;
            }
            public static bool CastTo(Vector3 from, Vector3 to, LayerMask layer, float radius, out RaycastHit hit, bool debug = false)
            {
                Ray ray = new Ray(from, (to - from).normalized);
                bool result = false;
                hit = new RaycastHit();
                var hits = Physics.SphereCastAll(ray, radius, Vector3.Distance(from, to), layer);
                if (hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].distance > 0 && (hits[i].distance < hit.distance || hit.distance == 0))
                        {
                            result = true;
                            hit = hits[i];
                            hit.distance += radius;
                        }
                    }
                }
                if (!result)
                {
                    hits = Physics.RaycastAll(ray, Vector3.Distance(from, to), layer);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].distance > 0 && (hits[i].distance < hit.distance || hit.distance == 0))
                        {
                            result = true;
                            hit = hits[i];
                        }
                    }
                }
                if (debug)
                {
                    EditorTools.DrawLineInEditor(from, to, Color.grey);
                    EditorTools.DrawSphereInEditor(result ? from + (ray.direction * hit.distance) : to, 0.05f, Color.grey);
                }
                return result;
            }
            public static bool CastTo2D(Vector2 from, Vector2 to, LayerMask layer, float radius, out RaycastHit2D hit, bool debug = false)
            {
                Vector2 dir = (to - from).normalized;
                bool result = false;
                hit = new RaycastHit2D();
                var hits = Physics2D.CircleCastAll(from, radius, dir, Vector3.Distance(from, to), layer);
                if (hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].distance > 0 && (hits[i].distance < hit.distance || hit.distance == 0))
                        {
                            result = true;
                            hit = hits[i];
                            hit.distance += radius;
                        }
                    }
                }
                if (!result)
                {
                    hits = Physics2D.RaycastAll(from, dir, Vector2.Distance(from, to), layer);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].distance > 0 && (hits[i].distance < hit.distance || hit.distance == 0))
                        {
                            result = true;
                            hit = hits[i];
                        }
                    }
                }
                if (debug)
                {
                    EditorTools.DrawLineInEditor(from, to, Color.grey);
                    EditorTools.DrawSphereInEditor(result ? from + (dir * hit.distance) : to, 0.05f, Color.grey);
                }
                return result;
            }
            #endregion Casts
            #region Async
            public static async Task MoveTo(Transform transform, Vector3 destination, float speed = 1f)
            {
                Vector3 basePosition = transform.position;
                float max = speed * 200; // 1000 (1s) / 5
                for (int i = 0; i <= max; i++)
                {
                    transform.position = Vector3.Lerp(basePosition, destination, i / max);
                    await Task.Delay(5);
                }
                return;
            }
            public static async Task MoveTo(Transform transform, Vector3 destination, Quaternion destinationRotation, float speed = 1f)
            {
                Quaternion baseRotation = transform.rotation;
                Vector3 basePosition = transform.position;
                float max = speed * 200; // 1000 (1s) / 5
                for (int i = 0; i <= max; i++)
                {
                    transform.position = Vector3.Lerp(basePosition, destination, i / max);
                    transform.rotation = Quaternion.Lerp(baseRotation, destinationRotation, i / max);
                    await Task.Delay(5);
                }
                return;
            }
            public static async Task MoveTo(Transform transform, Vector3[] points, float speed = 1f)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    await MoveTo(transform, points[i], speed);
                }
                return;
            }
            public static async Task MoveTo(Transform transform, Vector3[] points, Quaternion[] rotations, float speed = 1f)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    await MoveTo(transform, points[i], rotations[i], speed);
                }
                return;
            }
            #endregion Async
            #region IsInCamera
            public static bool IsInCamera(Vector3 vector, float perc)
            {
                return Camera.allCameras[0].WorldToScreenPoint(vector).IsBetween(new Vector3() + (Camera.allCameras[0].pixelRect.size.ToVector3() * perc), Camera.allCameras[0].pixelRect.size - (Camera.allCameras[0].pixelRect.size * perc));
            }

            public static bool IsInCamera(Transform transform, float perc)
            {
                return Camera.allCameras[0].WorldToScreenPoint(transform.position).IsBetween(new Vector3() + (Camera.allCameras[0].rect.size.ToVector3() * perc), Camera.allCameras[0].rect.size - (Camera.allCameras[0].rect.size * perc));
            }
            #endregion IsInCamera
        }
        #region Timer
        public class Timer
        {
            public static bool MinimumDelay(int id, int duration, bool firstTryPasses = false, bool clearOnSuccess = true)
            {
                var c = timers.FindIndex(t => t.Key == id);
                if (c != -1)
                {
                    var t = timers.Find(t => t.Key == id);
                    if (t.Value.ElapsedMilliseconds > duration)
                    {

                        if (clearOnSuccess)
                        {
                            t.Value.Stop();
                            timers.RemoveAt(c);
                        }
                        else
                        {
                            t.Value.Restart();
                        }
                        return true;
                    }
                    return false;
                }
                else
                {
                    Stopwatch r = new Stopwatch();
                    r.Start();
                    timers.Add(new KeyValuePair<int, Stopwatch>(id, r));
                    if (duration == 0)
                        return true;
                    return firstTryPasses;

                }
            }
            public static void ClearDelay(int id)
            {
                var c = timers.FindIndex(t => t.Key == id);
                if (c != -1)
                {
                    var t = timers.Find(t => t.Key == id);
                    t.Value.Stop();
                    timers.RemoveAt(c);
                }
            }
            public delegate void TimerCallback();
            private static readonly List<KeyValuePair<int, Stopwatch>> timers = new List<KeyValuePair<int, Stopwatch>>();
            public static void StartTimer(int id)
            {
                Stopwatch r = new Stopwatch();
                r.Start();
                timers.Add(new KeyValuePair<int, Stopwatch>(id, r));
            }
            public static void StartTimer(int id, int delay, TimerCallback callback, bool loop = true)
            {
                Stopwatch r = new Stopwatch();
                r.Start();
                timers.Add(new KeyValuePair<int, Stopwatch>(id, r));
                HandleCallback(r, delay, callback, loop);
            }
            public static void StartTimer<T>(int id, int delay, Action<T> callback, T value, bool loop = true)
            {
                Stopwatch r = new Stopwatch();
                r.Start();
                timers.Add(new KeyValuePair<int, Stopwatch>(id, r));
                HandleCallback(r, delay, callback, value, loop);
            }
            private async static void HandleCallback(Stopwatch stopwatch, int delay, TimerCallback callback, bool loop)
            {
                int objectiveDelay = delay;
                while (stopwatch.IsRunning)
                {
                    await Task.Delay(5);
                    if (stopwatch.ElapsedMilliseconds < objectiveDelay)
                        continue;
                    if (stopwatch.IsRunning)
                    {
                        callback.Invoke();
                        if (!loop)
                        {
                            stopwatch.Stop();
                            timers.Remove(timers.Find(t => t.Value == stopwatch));
                            break;
                        }
                        objectiveDelay += delay;
                    }
                }
            }
            private async static void HandleCallback<T>(Stopwatch stopwatch, int delay, Action<T> callback, T value, bool loop)
            {
                int objectiveDelay = delay;
                while (stopwatch.IsRunning)
                {
                    await Task.Delay(5);
                    if (stopwatch.ElapsedMilliseconds < objectiveDelay)
                        continue;
                    if (stopwatch.IsRunning)
                    {
                        callback.Invoke(value);
                        if (!loop)
                        {
                            stopwatch.Stop();
                            timers.Remove(timers.Find(t => t.Value == stopwatch));
                            break;
                        }
                        objectiveDelay += delay;
                    }
                }
            }
            public static void RestartTimer(int id)
            {
                timers.Find(t => t.Key == id).Value.Restart();
            }
            public static long GetTime(int id)
            {
                return timers.Find(t => t.Key == id).Value.ElapsedMilliseconds;
            }
            public static Stopwatch GetTimer(int id)
            {
                return timers.Find(t => t.Key == id).Value;
            }
            public static void EndTimer(int id)
            {
                var r = timers.Find(t => t.Key == id);
                if (!r.Equals(default(KeyValuePair<int, Stopwatch>)))
                {
                    r.Value.Stop();
                    timers.RemoveAll(t => t.Key == id);
                }
            }
            public static void StopTimer(int id)
            {
                EndTimer(id);
            }
        }
        #endregion Timer
        #region Slider
        [Serializable]
        public class Slider
        {
            [SerializeField, LabelOverride("Start Value")] private double _currentValue;
            [SerializeField, LabelOverride("Number of tiles")] private int _tilesCount;
            [SerializeField, LabelOverride("Tile Width")] private double _tileSize;
            [SerializeField, LabelOverride("Scrollrect")] private ScrollRect _scrollBar;
            [SerializeField] private double _speed;
            [SerializeField] private double _deceleration;
            [SerializeField] private double _deformation;
            [SerializeField] private bool _limitDeformation = true;
            private double _currentSpeed;

            public double CurrentValue { get => _currentValue; set => _currentValue = value; }
            public int TileCount { get => _tilesCount; set => _tilesCount = value; }
            public double TileSize { get => _tileSize; set => _tileSize = value; }
            public ScrollRect ScrollBar { get => _scrollBar; set => _scrollBar = value; }
            public double Speed { get => _speed; set => _speed = value; }
            public double Deceleration { get => _deceleration; set => _deceleration = value; }
            public double Deformation { get => _deformation; set => _deformation = value; }
            public double CurrentSpeed { get => _currentSpeed; }

            public bool LimitDeformation { get => _limitDeformation; set => _limitDeformation = value; }

            public double TilesPerScreen
            {
                get
                {
                    return _scrollBar.viewport.parent.GetComponent<RectTransform>().sizeDelta.x / _tileSize;
                }
            }

            public float IdealSize
            {
                get
                {
                    return (float)((_tilesCount + TilesPerScreen - 1) * _tileSize);
                }
            }

            public void ExternalInitialize()
            {
                _scrollBar.horizontalNormalizedPosition = (float)_currentValue;
            }

            public Slider(ScrollRect scrollbar, double speed, double deceleration, double tileSize, int tilesCount, double deformation, double startValue = 0.5, bool limitDeformation = true)
            {
                _scrollBar = scrollbar;
                _speed = speed;
                _deceleration = deceleration;
                _tileSize = tileSize;
                _tilesCount = tilesCount;
                _deformation = deformation;
                _currentValue = startValue;
                _scrollBar.horizontalNormalizedPosition = (float)startValue;
                _limitDeformation = limitDeformation;
            }
            public int GetCurrentItem()
            {
                return (int)Math.Round((_tilesCount - 1) * _currentValue);
            }
            public double GetSlide()
            {
                return _currentValue;
            }
            public double GetSlidingSpeed()
            {
                return _currentSpeed;
            }
            public void Slide(double impulse)
            {
                _currentSpeed = Math.Max(-1, Math.Min(1, _currentSpeed + (impulse * _speed)));
            }
            public double[] GetTileSizes()
            {
                KeyValuePair<int, double>[] res = new KeyValuePair<int, double>[_tilesCount];
                if (res.Length > 1)
                {
                    for (int i = 0; i < res.Length; i++)
                    {
                        res[i] = new KeyValuePair<int, double>(i, Math.Abs(_tileSize * (i - ((res.Length - 1) * _currentValue))));
                    }
                    res = res.OrderBy(t => t.Value).ToArray();
                    KeyValuePair<int, double>[] result = new KeyValuePair<int, double>[_tilesCount];
                    for (int i = 0; i < res.Length; i++)
                    {
                        result[i] = new KeyValuePair<int, double>(res[i].Key, _tileSize * (_limitDeformation ? Math.Max(_deformation, Math.Pow(_deformation, res[i].Value / _tileSize)) : Math.Pow(_deformation, res[i].Value / _tileSize)));
                    }
                    result = result.OrderBy(t => t.Key).ToArray();
                    return result.Select(t => t.Value).ToArray();
                }
                else
                {
                    return _tileSize.MakeArray();
                }

            }
            public void Update()
            {
                if (_currentSpeed != 0)
                {
                    _scrollBar.horizontalNormalizedPosition = (float)_currentValue;
                }
                else
                {
                    if (_scrollBar.horizontalNormalizedPosition != _currentValue)
                    {
                        _currentValue = _scrollBar.horizontalNormalizedPosition;
                    }
                }
                _currentValue = Math.Max(0, Math.Min(1, _currentValue + (_currentSpeed * Time.smoothDeltaTime)));
                if (_currentSpeed > 0)
                {
                    _currentSpeed = Math.Max(0, _currentSpeed - (_deceleration * Time.smoothDeltaTime));
                }
                else
                {
                    _currentSpeed = Math.Min(0, _currentSpeed + (_deceleration * Time.smoothDeltaTime));
                }
            }
        }
        #endregion Slider
        #endregion Utilities
        #region EditorTools
        public class EditorTools
        {
            #region Drawers
            public static void DrawLineInEditor(Vector3 from, Vector3 to, Color color)
            {
#if (UNITY_EDITOR)
                if (!Application.isPlaying)
                {
                    Gizmos.color = color;
                    Gizmos.DrawLine(from, to);
                }
#endif
            }
            public static void DrawSphereInEditor(Vector3 position, float radius, Color color)
            {
#if (UNITY_EDITOR)
                if (!Application.isPlaying)
                {
                    Gizmos.color = color;
                    Gizmos.DrawSphere(position, radius);
                }
#endif
            }
            public static void DrawCubeInEditor(Vector3 position, Vector3 dimensions, Color color)
            {
#if (UNITY_EDITOR)
                if (!Application.isPlaying)
                {
                    Gizmos.color = color;
                    Gizmos.DrawCube(position, dimensions * 2);
                }
#endif
            }
            #endregion Drawers
        }
        #endregion EditorTools
    }
    namespace CameraAndNodes
    {
        public class Nodes
        {
            #region Camera Nodes
            public static KeyValuePair<Vector3, Quaternion> CalculateNode(Vector3 defaultPos, Quaternion defaultRot, Vector3 playerPos, NodeContent node)
            {
                Vector3 pos = new Vector3();
                Quaternion rot = new Quaternion();
                if (node.type == NodeType.Relative)
                {
                    pos = defaultPos + node.position;
                    rot = defaultRot * node.rotation.Value;
                }
                else if (node.type == NodeType.AbsolutePosition)
                {
                    pos = playerPos + node.position;
                    rot = defaultRot * node.rotation.Value;
                }
                else if (node.type == NodeType.AbsoluteRotation)
                {
                    pos = defaultPos + node.position;
                    rot = node.rotation.Value;
                }
                else if (node.type == NodeType.Absolute)
                {
                    pos = playerPos + node.position;
                    rot = node.rotation.Value;
                }
                else if (node.type == NodeType.Coordinates)
                {
                    pos = node.position;
                    rot = node.rotation.Value;
                }
                return new KeyValuePair<Vector3, Quaternion>(pos, rot);
            }

            public static KeyValuePair<Vector3, NodeContent> FindNodes(Vector3 pos)
            {
                var r = UnityEngine.Object.FindObjectsOfType<Node>();
                for (int i = r.Length - 1; i >= 0; i--)
                {
                    if (Vector3.Distance(r[i].transform.position, pos) > r[i].Content.range)
                    {
                        r.RemoveAt(i);
                    }
                }
                return new KeyValuePair<Vector3, NodeContent>();
            }

            public static KeyValuePair<Vector3[], Node[]> FindNodes(Vector3 pos, float range)
            {
                var r = UnityEngine.Object.FindObjectsOfType<Node>();
                for (int i = r.Length - 1; i >= 0; i--)
                {
                    if (Vector3.Distance(r[i].transform.position, pos) > range)
                    {
                        r.RemoveAt(i);
                    }
                }
                return new KeyValuePair<Vector3[], Node[]>(r.Select(t => t.transform.position).ToArray(), r);
            }
            #endregion Camera Nodes

        }
        #region Definitions
        [Serializable]
        public struct NodeContent
        {
            public NodeType type;
            public Vector3 position;
            public SerializableQuaternion rotation;
            public float range;
            public bool isFixed;
            public SerializableQuaternion rotationFixed;
            public void Update()
            {
                rotation.Update();
                rotationFixed.Update();
            }
            public void Default()
            {
                rotation = new SerializableQuaternion();
                rotationFixed = new SerializableQuaternion();
            }
        }
        [Serializable]
        public struct TPSNodeContent
        {
            public Vector2 offset;
            public float fakeMiddle;
            public float distance;
            public float range;
            public void Default()
            {
                offset = new Vector2();
                distance = 2;
                range = 1;
            }
        }

        [Serializable]
        public enum NodeType
        {
            Relative,
            AbsolutePosition,
            AbsoluteRotation,
            Absolute,
            Coordinates
        }

        [Serializable]
        public struct NodeHelpSettings
        {
            public bool ShowRange;
            public bool ShowSafeRange;
            public bool ShowCamPreview;
            public bool ShowNodes;
            public bool ShowNodeDirection;
            public bool ShowNodePredictiveDirection;
            public bool ShowNodePaths;
            public bool AlwaysShowNodeDirection;
            public bool ShowSelection;
            public bool ShowRectangularDebug;
            public bool ShowCameraBoom;
            public NodeHelpSettings(bool showRange = true, bool showSafeRange = true, bool showCamPreview = true, bool showNodes = true, bool showNodeDirection = true, bool showNodePredictiveDirection = true, bool showNodePaths = true, bool alwaysShowNodeDirection = false, bool showSelection = true, bool showRectangularDebug = false, bool showCameraBoom = false)
            {
                ShowRange = showRange;
                ShowSafeRange = showSafeRange;
                ShowCamPreview = showCamPreview;
                ShowNodes = showNodes;
                ShowNodeDirection = showNodeDirection;
                ShowNodePredictiveDirection = showNodePredictiveDirection;
                ShowNodePaths = showNodePaths;
                AlwaysShowNodeDirection = alwaysShowNodeDirection;
                ShowSelection = showSelection;
                ShowRectangularDebug = showRectangularDebug;
                ShowCameraBoom = showCameraBoom;
            }
        }
        #endregion Definitions
    }
    #region Definitions
    #region Serializable Quaternion
    [Serializable]
    public class SerializableQuaternion
    {
        [SerializeField]
        private Vector3 _angle = new Vector3();
        public Vector3 Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                _angle = value;
                _q = Quaternion.Euler(value);
            }
        }
        private Quaternion _q = Quaternion.identity;
        public Quaternion Value
        {
            get
            {
                return _q;
            }
            set
            {
                _q = value;
                _angle = value.eulerAngles;
            }
        }

        public SerializableQuaternion(Vector3 angle)
        {
            _q = Quaternion.Euler(angle);
            _angle = angle;
        }

        public SerializableQuaternion(Quaternion angle)
        {
            _q = angle;
            _angle = angle.eulerAngles;
        }

        public SerializableQuaternion()
        {
            _q = Quaternion.identity;
            _angle = Quaternion.identity.eulerAngles;
        }

        public void Update()
        {
            _q = Quaternion.Euler(Angle);
        }
    }
    #endregion Serializable Quaternion
    #endregion Definitions
}