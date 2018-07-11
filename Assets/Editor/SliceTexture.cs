﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SliceTexture
{
    private static List<int> curRect;
    private static int rectLength;
    [MenuItem("YKTools/Sprite/SliceTexture")]
    static void Process()
    {
        // 选择图片
        Texture2D selectionImg = Selection.activeObject as Texture2D;
        // 临时赋值
        //int[] sliceDataArr = new int[]{ 0, 0, 64, 64, 1,
        //    0, 64, 64, 16, 1,
        //    64, 64, 64, 8, 1,
        //    64, 72, 64, 8, 1,
        //    0, 80, 16, 16, 12,
        //    0, 96, 16, 16, 12,
        //    0, 112, 32, 48, 8,
        //    0, 160, 32, 48, 8,
        //    0, 208, 32, 48, 8 };//pl00
        //int[] sliceDataArr = new int[]{ 0, 0, 32, 32, 5,
        //    160, 0, 16, 16, 1,
        //    192, 0, 32, 32, 1,
        //    0, 32, 16, 16, 16,
        //    0, 48, 32, 32, 4,
        //    0, 80, 32, 32, 4,
        //    0, 112, 64, 64, 2,
        //    128, 48, 128, 128, 1,
        //    0, 176, 64, 64, 4,
        //    0,240,16,16,16}; // etama2  
        //int[] sliceDataArr = new int[]{ 0, 64, 64, 64, 8,
        //    0, 128, 32, 32, 12,
        //    0, 160, 32, 32, 12,
        //    0, 192, 32, 32, 12,
        //    0, 224, 32, 32, 12,
        //    384, 128, 64, 64, 2,
        //    384, 192, 64, 64, 2}; // enemy5
        //int[] sliceDataArr = new int[]{ 0, 0, 8, 8, 8,
        //    0, 8, 8, 8, 8,
        //    64, 0, 16, 16, 10,
        //    248, 0, 8, 8, 1,
        //    0, 16, 32, 32, 8,
        //    0, 48, 8, 8, 8,
        //    0, 56, 8, 8, 8,
        //    208, 48, 16, 16, 3,
        //    0, 64, 16, 16, 16,
        //    0, 80, 16, 16, 16,
        //    0, 96, 16, 16, 16,
        //    0, 112, 16, 16, 16,
        //    0, 128, 16, 16, 16,
        //    0, 144, 16, 16, 16,
        //    0, 160, 16, 16, 16,
        //    0, 176, 16, 16, 16,
        //    0, 192, 16, 16, 16,
        //    0, 208, 16, 16, 16,
        //    0, 224, 16, 16, 16,
        //    0, 240, 16, 16, 16,}; // etama
        //int[] sliceDataArr = new int[]{ 0, 16, 32, 32, 4,
        //    0, 48, 16, 16, 16,
        //    0, 64, 32, 24, 6,
        //    0, 88, 32, 24, 6,
        //    0, 112, 24, 16, 6,
        //    0, 128, 24, 16, 6,
        //    0, 144, 16, 16, 16,
        //    0, 160, 64, 64, 4,
        //    0, 224, 32, 32, 8,}; //etama8
        //int[] sliceDataArr = new int[]{ 0, 0, 64, 64, 4,
        //    0, 64, 32, 32, 8,
        //    0, 96, 32, 32, 8,
        //    0, 128, 32, 32, 8,
        //    0, 160, 32, 32, 8,
        //    0, 192, 32, 32, 8,
        //    0, 224, 32, 32, 8,}; //etama6
        int[] sliceDataArr = new int[]{ 0, 0, 64, 64, 8,
            0, 64, 32, 32, 12,
            0, 96, 32, 32, 12,
            0, 128, 32, 32, 12,
            0, 160, 32, 32, 12,
            0, 192, 32, 32, 8,
            0, 224, 32, 32, 8,
            384,64,64,64,2,
            384,128,64,64,2,
            320,224,48,48,4,
            320,272,48,48,4,
            320,320,48,48,4,
            320,368,48,48,4,
            320,416,48,48,4,
            320,464,48,48,4,
            0,320,48,32,4,
            0,352,48,32,4,
            0,384,48,32,4,
            0,416,48,32,4,
            0,448,48,32,4,
            0,480,48,32,4,}; //enemy
        List<int> sliceData = new List<int>(sliceDataArr);
        ProcessSlicing(selectionImg, sliceData);
        //创建导出路径
        //AssetDatabase.CreateFolder(rootPath,selectionImg.name);
    }

    private static void Init()
    {
        if ( curRect == null )
        {
            curRect = new List<int>();
        }
        else
        {
            curRect.Clear();
        }
        rectLength = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="metaData">起始X坐标，起始Y坐标，宽，高，数量</param>
    private static void ProcessSlicing(Texture2D texture,List<int> sliceData)
    {
        int width = texture.width;
        int height = texture.height;
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;
        int tmpLen = sliceData.Count / 5;
        int sliceCount = 0;
        int i, j,rectCount,startX,startY,tmpWidth,tmpHeight,tmpIndex;
        // 先计算总切割数
        for (i=0;i<tmpLen;i++)
        {
            j = i * 5;
            sliceCount += sliceData[j + 4];
        }
        SpriteMetaData[] datas = new SpriteMetaData[sliceCount];
        SpriteMetaData tmpData;
        int id = 0;
        // 循环获取切割的矩形数据
        for (i=0;i<tmpLen;i++)
        {
            tmpIndex = i * 5;
            startX = sliceData[tmpIndex];
            startY = sliceData[tmpIndex + 1];
            tmpWidth = sliceData[tmpIndex + 2];
            tmpHeight = sliceData[tmpIndex + 3];
            rectCount = sliceData[tmpIndex + 4];
            for (j=0;j<rectCount;j++)
            {
                tmpData = new SpriteMetaData();
                tmpData.name = texture.name + "_" + id;
                tmpData.rect = new Rect(startX,startY,tmpWidth,tmpHeight);
                datas[id] = tmpData;
                // 更新
                id++;
                startX += tmpWidth;
            }
        }
        texImp.spritesheet = datas;
        texImp.fadeout = !texImp.fadeout;
        texImp.SaveAndReimport();
        texImp.fadeout = false;
        texImp.SaveAndReimport();
    }
}
