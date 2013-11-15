using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace LLT
{
	public sealed class CoreTexture2D
	{
	    private sealed class PackNode
	    {
	        private CoreRect _rect;
	        private PackNode[] _childs;
	        private int _padding;
	        private CoreTexture2D _texture2D;
	
	        public CoreRect Rect
	        {
	            get
	            {
	                return _rect;
	            }
	        }
	
	        public bool HasTexture
	        {
	            get
	            {
	                return _texture2D != null;
	            }
	        }
	
	        public CoreTexture2D Texture
	        {
	            get
	            {
	                return _texture2D;
	            }
	        }
	
	        public PackNode(CoreRect rect, int padding)
	        {
	            _rect = rect;
	            _padding = padding;
	        }
	        public static PackNode GrowFromRoot(PackNode oldRoot, int growX, int growY)
	        {
	            if (oldRoot._rect.Width == 0 && oldRoot._rect.Height == 0)
	            {
	                int newSizeX = CoreMath.NextPowerOfTwo(growX);
	                int newSizeY = CoreMath.NextPowerOfTwo(growY);
	                return new PackNode(new CoreRect(0, 0, newSizeX, newSizeY), oldRoot._padding);
	            }
	            else
	            {
	                float newSizeX = oldRoot._rect.Width;
	                float newSizeY = oldRoot._rect.Height;
	                if (growY > oldRoot._rect.Height || growX > oldRoot._rect.Width)
	                {
	                    if (growY > oldRoot._rect.Height)
	                    {
	                        //Grow heigh-wise
	                        newSizeY = oldRoot._rect.Height + growY + oldRoot._padding;
	                    }
	                    if (growX > oldRoot._rect.Width)
	                    {
							 //Grow width-wise
	                        newSizeX = oldRoot._rect.Width + growX + oldRoot._padding;
	                    }
	                }
	                else
	                {
	                    if (oldRoot._rect.Width > oldRoot._rect.Height)
	                    {
	                        //Grow heigh-wise
	                        newSizeY = oldRoot._rect.Height + growY + oldRoot._padding;
	                    }
	                    else
	                    {
	                        //Grow width-wise
	                        newSizeX = oldRoot._rect.Width + growX + oldRoot._padding;
	                    }
	                }
	                newSizeX = CoreMath.NextPowerOfTwo((int)newSizeX);
	                newSizeY = CoreMath.NextPowerOfTwo((int)newSizeY);
	                if (newSizeX > _maxTextureSize || newSizeY > _maxTextureSize)
	                {
	                    return null;
	                }
	                var newRoot = new PackNode(new CoreRect(0, 0, newSizeX, newSizeY), oldRoot._padding);
	                newRoot._childs = new PackNode[2];
	                newRoot._childs[0] = new PackNode(new CoreRect(0, 0, newSizeX, oldRoot._rect.Height), oldRoot._padding);
	                newRoot._childs[0]._childs = new PackNode[2];
	                newRoot._childs[0]._childs[0] = oldRoot;
	                newRoot._childs[0]._childs[1] = new PackNode(new CoreRect(oldRoot._rect.Width + oldRoot._padding, 0, newSizeX - oldRoot._rect.Width - oldRoot._padding, oldRoot._rect.Height), oldRoot._padding);
	                newRoot._childs[1] = new PackNode(new CoreRect(0, oldRoot._rect.Height + oldRoot._padding, newSizeX, newSizeY - oldRoot._rect.Height - oldRoot._padding), oldRoot._padding);
	                return newRoot;
	            }
	        }
	
	        public bool Insert(CoreTexture2D texture2D)
	        {
	            if(texture2D.Width > _maxTextureSize || texture2D.Height > _maxTextureSize)
	            {
	                if(_texture2D == null)
	                {
	                    _rect = new CoreRect(0, 0, texture2D.Width, texture2D.Height);
	                    _texture2D = texture2D;
	                    return true;
	                }
	                else
	                {
	                    return false;
	                }
	            }
	
	            // Try inserting in childs.
	            if(_childs != null)
	            {
	                return _childs[0].Insert(texture2D) ? true : _childs[1].Insert(texture2D);
	            }
	
	            // Check if it`s the same texture.
	            if(_texture2D != null )
	            {
	                if(_texture2D== texture2D)
	                {
	                    return true;
	                }
	                return false;
	            }
	
	            // Is too big for actual rect.
	            if (_rect.Width < texture2D.Width || _rect.Height < texture2D.Height)
	            {
	                return false;
	            }
	
	            // Is perfect fit insert.
	            if (_rect.Width == texture2D.Width && _rect.Height == texture2D.Height)
	            {
	                _texture2D = texture2D;
	                return true;
	            }
	
	            // Create new childs and try inserting.
	            _childs = new PackNode[2];
	            if (_rect.Width - texture2D.Width < _rect.Height - texture2D.Height)
	            {
	                _childs[0] = new PackNode(new CoreRect(_rect.X, _rect.Y, _rect.Width, texture2D.Height), _padding);
	                _childs[1] = new PackNode(new CoreRect(_rect.X, _rect.Y + texture2D.Height + _padding, _rect.Width, _rect.Height - texture2D.Height - _padding), _padding);
	            }
	            else
	            {
	                _childs[0] = new PackNode(new CoreRect(_rect.X, _rect.Y, texture2D.Width, _rect.Height), _padding);
	                _childs[1] = new PackNode(new CoreRect(_rect.X + texture2D.Width + _padding, _rect.Y, _rect.Width - texture2D.Width - _padding, _rect.Height), _padding);
	            }
	
	            return _childs[0].Insert(texture2D);
	        }
	
	        public List<PackNode> PackNodes()
	        {
	            List<PackNode> packNodes = new List<PackNode>();
	            packNodes.Add(this);
	            if (_childs != null)
	            {
	                packNodes.AddRange(_childs[0].PackNodes());
	                packNodes.AddRange(_childs[1].PackNodes());
	            }
	            return packNodes;
	        }
	    }
	
	    public static Func<string, CoreTexture2D> PngDecoder;
	    public static Action<string, CoreTexture2D> PngEncoder;
	    private static int _maxTextureSize = 1 << 15;
	
	    private int _width;
	    private int _height;	
	    private int[] _argb;

	    public int Width
	    {
	        get
	        {
	            return _width;
	        }
	    }
	
	    public int Height
	    {
	        get
	        {
	            return _height;
	        }
	    }
	
	    public int[] ARGB
	    {
	        get
	        {
	            return _argb;
	        }
	    }
	
	    public CoreTexture2D(int width, int height, int[] argb)
	    {
	        _width = width;
	        _height = height;
	        _argb = argb;
	    }
	
	    public CoreTexture2D(int width, int height)
	    {
	        _width = width;
	        _height = height;
	        _argb = new int[width * height];
	    }
	
	    public CoreTexture2D(string path)
	    {
	        CoreAssert.Fatal(Path.GetExtension(path) == ".png" && PngDecoder != null);
	        var texture2D = PngDecoder(path);
	
	        _width = texture2D.Width;
	        _height = texture2D.Height;
	        _argb = texture2D.ARGB;
	    }
	
	    public void SetPixels(int x, int y, CoreTexture2D other)
	    {
	        CoreAssert.Fatal(x + other.Width < Width && y + other.Height < Height, "Texture does not fit.");
	        for(var j = 0; j < other.Height; j++)
	        {
	            for (var i = 0; i < other.Width; i++)
	            {
	                _argb[(j + y) * Width + i + x] = other.ARGB[j * other.Width + i];
	            }
	        }
	    }
	
	    public void Save(string path)
	    {
	        CoreAssert.Fatal(Path.GetExtension(path) == ".png" && PngEncoder != null);
	        PngEncoder(path, this);
	    }
	
	    public static CoreRect[] Pack(CoreTexture2D[] originals, out CoreTexture2D atlas, int padding)
	    {
	        var textures = originals.OrderByDescending((x) => 2 * x.Width + 2 * x.Height).ToArray();
	        var root = new PackNode(new CoreRect(0, 0, 0, 0), 2);
	
	        for (var index = 0; index < textures.Length; index++)
	        {
	            var texture2D = textures[index];
	
	            if (!root.Insert(texture2D))
	            {
	                // Try growing this root otherwise create a new one.
	                var grown = PackNode.GrowFromRoot(root, texture2D.Width, texture2D.Height);
	                if (grown == null)
	                {
	                    throw new System.Exception("Atlas is full");
	                }
	                else
	                {
	                    // Reassign root and insert.
	                    root = grown;
	                    if(!root.Insert(texture2D))
	                    {
	                        throw new System.Exception("Problem inserting texture");
	                    }
	                }
	            }
	        }
	
	        int width = CoreMath.NextPowerOfTwo(root.Rect.Width);
	        int height = CoreMath.NextPowerOfTwo(root.Rect.Height);
	        var rects = new CoreRect[textures.Length];
	
	        atlas = new CoreTexture2D(width, height);
	        var packNodes = root.PackNodes();
	
	        foreach (PackNode packNode in packNodes)
	        {
	            if(!packNode.HasTexture)
	                continue;
	
	            atlas.SetPixels((int)packNode.Rect.X, (int)packNode.Rect.Y, packNode.Texture);
	            rects[Array.IndexOf(originals, packNode.Texture)] = new CoreRect((packNode.Rect.X + padding) / (float)atlas.Width, (packNode.Rect.Y+padding)/ (float)atlas.Height, (packNode.Texture.Width-2*padding)/(float)atlas.Width, (packNode.Texture.Height-2*padding)/(float)atlas.Height);
	        }
	
	        return rects;
	    }
	}
}