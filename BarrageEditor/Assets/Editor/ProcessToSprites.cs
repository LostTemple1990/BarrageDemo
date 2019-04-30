using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;

public static class ProcessToSprites
{
    [MenuItem("YKTools/Sprite/Process to Sprites")]
    static void ProcessToSprite()
    {
        // 获取选择的位图
        Texture2D texture = Selection.activeObject as Texture2D;
        string path = AssetDatabase.GetAssetPath(texture);
        string rootPath = Path.GetDirectoryName(path);//获取路径名称

        TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;//获取图片入口

        //检测文件夹是否存在
        if ( !Directory.Exists(rootPath+"/"+texture.name) )
        {
            AssetDatabase.CreateFolder(rootPath, texture.name);//创建文件夹
        }

        int tmpWidth, tmpHeight;
        int startPosX,startPosY,endPosX,endPosY;
        Color curPixel;
        foreach (SpriteMetaData metaData in texImp.spritesheet)//遍历小图集
        {
            tmpWidth = (int)metaData.rect.width;
            tmpHeight = (int)metaData.rect.height;
            Texture2D myimage = new Texture2D(tmpWidth, tmpHeight);

            //abc_0:(x:2.00, y:400.00, width:103.00, height:112.00)
            startPosX = (int)metaData.rect.x;
            startPosY = (int)metaData.rect.y;
            endPosX = startPosX + tmpWidth;
            endPosY = startPosY + tmpHeight;
            for (int y = startPosY; y < endPosY; y++)//Y轴像素
            {
                for (int x = startPosX; x < endPosX; x++)
                {
                    curPixel = texture.GetPixel(x, y);
                    myimage.SetPixel(x - startPosX, y - startPosY, curPixel);
                }
            }


            //转换纹理到EncodeToPNG兼容格式
            if (myimage.format != TextureFormat.ARGB32 && myimage.format != TextureFormat.RGB24)
            {
                Texture2D newTexture = new Texture2D(myimage.width, myimage.height);
                newTexture.SetPixels(myimage.GetPixels(0), 0);
                myimage = newTexture;
            }
            var pngData = myimage.EncodeToPNG();
            //AssetDatabase.CreateAsset(myimage, rootPath + "/" + image.name + "/" + metaData.name + ".PNG");
            File.WriteAllBytes(rootPath + "/" + texture.name + "/" + metaData.name + ".PNG", pngData);
            // 刷新资源窗口界面
            AssetDatabase.Refresh();
        }
    }
}