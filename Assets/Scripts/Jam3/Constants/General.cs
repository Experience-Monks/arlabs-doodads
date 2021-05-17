//-----------------------------------------------------------------------
// <copyright file="General.cs" company="Jam3 Inc">
//
// Copyright 2021 Jam3 Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace Jam3
{
    /// <summary>
    /// Interactive type.
    /// </summary>
    public enum InteractiveType
    {
        Interactive = 0,
        Action = 1
    }

    /// <summary>
    /// Interactive status.
    /// </summary>
    public enum InteractiveStatus
    {
        None,
        Placement,
        Moving,
        Transform
    }

    /// <summary>
    /// Layers.
    /// </summary>
    public enum Layers
    {
        Default = 0,
        TransparentFX = 1,
        IgnoreRaycast = 2,
        Draggable = 3,
        Water = 4,
        UI = 5,
        ARSpace = 6,
        PlayerSpace = 7,
        Effects = 8,
        Surface = 9,
        DraggableAction = 10,
        HandlerX = 11,
        HandlerY = 12,
        HandlerZ = 13,
        BackgroundMesh = 14
    }

    /// <summary>
    /// Mouse buttons.
    /// </summary>
    public enum MouseButtons
    {
        Left,
        Right,
        Middle
    }

    /// <summary>
    /// Direction.
    /// </summary>
    public enum Direction
    {
        Left,
        Right,
        Forward,
        Backward,
    }
}
