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
    public class ImageData : ViewModelBase<ImageData>
    {

        public ImageData()
        {


        }

        private async void SetBitmap(byte[] bs)
        {
            var im = new InMemoryRandomAccessStream();
            var dr = new DataWriter(im);
            dr.WriteBytes(bs);
            await dr.StoreAsync();
            im.Seek(0);
            var bm = new BitmapImage();
            bm.SetSource(im);
            this.BitmapImage = bm;
        }


        public Byte[] ByteArray
        {
            get { return m_ByteArrayLocator(this).Value; }
            set
            {
                m_ByteArrayLocator(this).SetValueAndTryNotify(value);
                SetBitmap(value);

            }
        }

        #region Property Byte[] ByteArray Setup
        protected Property<Byte[]> m_ByteArray =
          new Property<Byte[]> { LocatorFunc = m_ByteArrayLocator };
        static Func<ViewModelBase, ValueContainer<Byte[]>> m_ByteArrayLocator =
            RegisterContainerLocator<Byte[]>(
                "ByteArray",
                model =>
                {
                    model.m_ByteArray =
                        model.m_ByteArray
                        ??
                        new Property<Byte[]> { LocatorFunc = m_ByteArrayLocator };
                    return model.m_ByteArray.Container =
                        model.m_ByteArray.Container
                        ??
                        new ValueContainer<Byte[]>("ByteArray", new byte[0], model);
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
            get { return m_BitmapImageLocator(this).Value; }
            set { m_BitmapImageLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property BitmapImage BitmapImage Setup
        protected Property<BitmapImage> m_BitmapImage =
          new Property<BitmapImage> { LocatorFunc = m_BitmapImageLocator };
        static Func<ViewModelBase, ValueContainer<BitmapImage>> m_BitmapImageLocator =
            RegisterContainerLocator<BitmapImage>(
                "BitmapImage",
                model =>
                {
                    model.m_BitmapImage =
                        model.m_BitmapImage
                        ??
                        new Property<BitmapImage> { LocatorFunc = m_BitmapImageLocator };
                    return model.m_BitmapImage.Container =
                        model.m_BitmapImage.Container
                        ??
                        new ValueContainer<BitmapImage>("BitmapImage", model);
                });
        #endregion



    }
}
