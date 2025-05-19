#version 330 core

uniform vec2 uResolution;
in vec3 vPosition;
out vec3 glPosition;
uniform float uCameraOffset;

void main()
{
    gl_Position = vec4(vPosition, 1.0);
    glPosition = vPosition;
}