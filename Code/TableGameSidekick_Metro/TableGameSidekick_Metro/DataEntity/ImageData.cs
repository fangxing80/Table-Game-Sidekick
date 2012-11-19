using MVVMSidekick.ViewModels;
using MVVMSidekick.Reactive;
using System.Reactive;
using System.Reactive.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.IO;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
namespace TableGameSidekick_Metro.DataEntity
{
    [DataContract]
    public class ImageData : BindableBase<ImageData>
    {

        


        public ImageData()
        {
            

        }


        private async void SetBitmap(byte[] bytes)
        {
            using (var im = await CreateStreamAsync(bytes))
            {
                var bm = new BitmapImage();
                bm.SetSource(im);
                this.BitmapImage = bm;
            };
        }

        private static async Task<InMemoryRandomAccessStream> CreateStreamAsync(byte[] bs)
        {
            var im = new InMemoryRandomAccessStream();
            var dr = new DataWriter(im);
            dr.WriteBytes(bs);
            await dr.StoreAsync();
            im.Seek(0);
            return im;
        }
        public async Task<InMemoryRandomAccessStream> GetStreamAsync()
        {
            return await CreateStreamAsync(ByteArray);

        }



        public Byte[] ByteArray
        {
            get { return _ByteArrayLocator(this).Value; }
            set
            {
                _ByteArrayLocator(this).SetValueAndTryNotify(value);
                SetBitmap(value);

            }
        }

        #region Property Byte[] ByteArray Setup
        protected Property<Byte[]> _ByteArray =
          new Property<Byte[]> { LocatorFunc = _ByteArrayLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<Byte[]>> _ByteArrayLocator =
            RegisterContainerLocator<Byte[]>(
            "ByteArray",
            model =>
            {
                model._ByteArray =
                    model._ByteArray
                    ??
                    new Property<Byte[]> { LocatorFunc = _ByteArrayLocator };
                return model._ByteArray.Container =
                    model._ByteArray.Container
                    ??
                    new ValueContainer<Byte[]>("ByteArray", model, new byte[0]);
            });
        #endregion

        /// <summary>
        /// 本属性不是作为绑定用 仅仅在序列化反序列化时使用
        /// </summary>
        [DataMember]
        public string Base64String
        {
            get { return Convert.ToBase64String(this.ByteArray); }
            set
            {
                ByteArray = Convert.FromBase64String(value);
                SetBitmap(ByteArray);
            }
        }




        public BitmapImage BitmapImage
        {
            get { return _BitmapImageLocator(this).Value; }
            set { _BitmapImageLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property BitmapImage BitmapImage Setup
        protected Property<BitmapImage> _BitmapImage =
          new Property<BitmapImage> { LocatorFunc = _BitmapImageLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<BitmapImage>> _BitmapImageLocator =
            RegisterContainerLocator<BitmapImage>(
            "BitmapImage",
            model =>
            {
                model._BitmapImage =
                    model._BitmapImage
                    ??
                    new Property<BitmapImage> { LocatorFunc = _BitmapImageLocator };
                return model._BitmapImage.Container =
                    model._BitmapImage.Container
                    ??
                    new ValueContainer<BitmapImage>("BitmapImage", model);
            });
        #endregion


        
       

    }
}
