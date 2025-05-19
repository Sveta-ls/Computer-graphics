#version 330 core
#define EPSILON 0.01 
#define BIG 1000000.0 
const int DIFFUSE = 1; 
const int REFLECTION = 2; 
const int REFRACTION = 3; 
const int DIFFUSE_REFLECTION = 1; 
const int MIRROR_REFLECTION = 2; 
const int MAX_DEPTH = 6;
uniform vec3 cubeColor;
uniform vec3 tetraColor;
uniform float mirrorCoef;
uniform float transparency;
uniform int maxDepth;
uniform float uCameraOffset;
uniform float uCameraOffset1;
uniform float uCameraOffset2;
uniform float RW;

in vec2 texCoord;

in vec3 glPosition;
out vec4 FragColor;

struct SCamera 
{ 
    vec3 Position; 
    vec3 View; 
    vec3 Up; 
    vec3 Side; 
    vec2 Scale; 
}; 
 
struct SRay 
{ 
    vec3 Origin; 
    vec3 Direction; 
};

struct STriangle
{
    vec3 v1;
    vec3 v2;
    vec3 v3;
    //vec3 color;
    int MaterialIdx;
};
struct SSphere 
{ 
    vec3 Center; 
    float Radius; 
    int MaterialIdx; 
}; 

struct SIntersection 
{ 
    float Time; 
    vec3 Point; 
    vec3 Normal; 
    vec3 Color; 
    vec4 LightCoeffs; 
    // 0 - non-reflection, 1 - mirror 
    float ReflectionCoef; 
    float RefractionCoef; 
    int MaterialType; 
}; 
struct SMaterial 
{ 
    //diffuse color 
    vec3 Color; 
    // ambient, diffuse and specular coeffs 
    vec4 LightCoeffs; 
    // 0 - non-reflection, 1 - mirror 
    float ReflectionCoef; 
    float RefractionCoef; 
    int MaterialType; 
}; 
struct SLight 
{ 
vec3 Position; 
}; 

struct STracingRay 
{ 
    SRay ray; 
    float contribution; 
    int depth; 
};


STriangle triangles[22]; 
SSphere spheres[2]; 
SMaterial materials[9];
SLight light; 
SCamera uCamera;

SCamera initializeDefaultCamera() 
{ 
    SCamera camera;
    camera.Position = vec3(uCameraOffset, uCameraOffset1, uCameraOffset2); 
    camera.View = vec3(0.0, 0.0, 1.0); 
    camera.Up = vec3(0.0, 1.0, 0.0); 
    camera.Side = vec3(1.0, 0.0, 0.0); 
    camera.Scale = vec2(1.0);
    return camera;
}

void initializeDefaultScene(out STriangle triangles[22], out SSphere spheres[2]) 
{ 
   light.Position = vec3(4.0, 2.0, -4.0); 

     //leftwall
     triangles[0].v1 = vec3(-5.0,-5.0,-5.0); 
     triangles[0].v2 = vec3(-5.0, 5.0, 5.0); 
     triangles[0].v3 = vec3(-5.0, 5.0,-5.0); 
     triangles[0].MaterialIdx = 2; 
 
     triangles[1].v1 = vec3(-5.0,-5.0,-5.0); 
     triangles[1].v2 = vec3(-5.0,-5.0, 5.0); 
     triangles[1].v3 = vec3(-5.0, 5.0, 5.0); 
     triangles[1].MaterialIdx = 2; 
     //rightwall
     triangles[4].v1 = vec3(5.0,-5.0,-5.0);
    triangles[4].v2 = vec3(5.0,5.0,-5.0);
    triangles[4].v3 = vec3(5.0,5.0,5.0);
    triangles[4].MaterialIdx = 0;
    
    triangles[5].v1 = vec3(5.0,-5.0,-5.0);
    triangles[5].v2 = vec3(5.0,5.0,5.0);
    triangles[5].v3 = vec3(5.0,-5.0,5.0);
    triangles[5].MaterialIdx = 0;
     //up
    triangles[8].v1 = vec3(-5.0,5.0,-5.0);
    triangles[8].v2 = vec3(5.0,5.0,-5.0);
    triangles[8].v3 = vec3(5.0,5.0,5.0);
    triangles[8].MaterialIdx = 3;
    
    triangles[9].v1 = vec3(-5.0,5.0,-5.0);
    triangles[9].v2 = vec3(5.0,5.0,5.0);
    triangles[9].v3 = vec3(-5.0,5.0,5.0);
    triangles[9].MaterialIdx = 3;

    //down
    triangles[6].v1 = vec3(-5.0,-5.0,-5.0);
    triangles[6].v2 = vec3(5.0,-5.0,-5.0);
    triangles[6].v3 = vec3(5.0,-5.0, 5.0);
    triangles[6].MaterialIdx = 3;

    triangles[7].v1 = vec3(-5.0,-5.0,-5.0);
    triangles[7].v2 = vec3(5.0,-5.0, 5.0);
    triangles[7].v3 = vec3(-5.0,-5.0, 5.0);
    triangles[7].MaterialIdx = 3;

    //back
     triangles[2].v1 = vec3(-5.0,-5.0, 5.0); 
    triangles[2].v2 = vec3( 5.0,-5.0, 5.0); 
    triangles[2].v3 = vec3(-5.0, 5.0, 5.0); 
    triangles[2].MaterialIdx = 1; 
    triangles[3].v1 = vec3( 5.0, 5.0, 5.0); 
    triangles[3].v2 = vec3(-5.0, 5.0, 5.0); 
    triangles[3].v3 = vec3( 5.0,-5.0, 5.0); 
    triangles[3].MaterialIdx = 1; 
     spheres[0].Center = vec3(-1.0,-1.0,-2.0); 
    spheres[0].Radius = 2.0; 
    spheres[0].MaterialIdx = 4; 
    spheres[1].Center = vec3(2.0,1.0,2.0); 
    spheres[1].Radius = 1.0; 
    spheres[1].MaterialIdx = 4; 
    spheres[0].MaterialIdx = 4; 
   
    //cube
    /*float cubeSize = 2.0;
    vec3 cubeCenter = vec3(3.0, 4.0, 2.0);
    int cubeMaterial = 6; 
    
    vec3 v0 = cubeCenter + vec3(-cubeSize/2, -cubeSize/2, -cubeSize/2);
    vec3 v1 = cubeCenter + vec3(-cubeSize/2, -cubeSize/2,  cubeSize/2);
    vec3 v2 = cubeCenter + vec3(-cubeSize/2,  cubeSize/2, -cubeSize/2);
    vec3 v3 = cubeCenter + vec3(-cubeSize/2,  cubeSize/2,  cubeSize/2);
    vec3 v4 = cubeCenter + vec3( cubeSize/2, -cubeSize/2, -cubeSize/2);
    vec3 v5 = cubeCenter + vec3( cubeSize/2, -cubeSize/2,  cubeSize/2);
    vec3 v6 = cubeCenter + vec3( cubeSize/2,  cubeSize/2, -cubeSize/2);
    vec3 v7 = cubeCenter + vec3( cubeSize/2,  cubeSize/2,  cubeSize/2);
    
   
    triangles[10] = STriangle(v1, v3, v5, cubeMaterial);
    triangles[11] = STriangle(v5, v3, v7, cubeMaterial);
   
    triangles[12] = STriangle(v0, v4, v2, cubeMaterial);
    triangles[13] = STriangle(v2, v4, v6, cubeMaterial);
  
    triangles[14] = STriangle(v0, v2, v1, cubeMaterial);
    triangles[15] = STriangle(v1, v2, v3, cubeMaterial);
    
  
    triangles[16] = STriangle(v4, v5, v6, cubeMaterial);
    triangles[17] = STriangle(v6, v5, v7, cubeMaterial);
   
    triangles[18] = STriangle(v0, v1, v4, cubeMaterial);
    triangles[19] = STriangle(v4, v1, v5, cubeMaterial);
  
    triangles[20] = STriangle(v2, v6, v3, cubeMaterial);
    triangles[21] = STriangle(v3, v6, v7, cubeMaterial);*/
} 

SRay GenerateRay(SCamera uCamera) 
{ 
    vec2 coords = glPosition.xy * uCamera.Scale; 
    vec3 direction = uCamera.View + uCamera.Side * coords.x + uCamera.Up * coords.y; 
    return SRay(uCamera.Position, normalize(direction));
}

bool IntersectSphere ( SSphere sphere, SRay ray, float start, float final, out float 
time ) 
{ 
    ray.Origin -= sphere.Center; 
    float A = dot ( ray.Direction, ray.Direction ); 
    float B = dot ( ray.Direction, ray.Origin ); 
    float C = dot ( ray.Origin, ray.Origin ) - sphere.Radius * sphere.Radius; 
    float D = B * B - A * C; 
    if ( D > 0.0 ) 
 { 
        D = sqrt ( D ); 
        //time = min ( max ( 0.0, ( -B - D ) / A ), ( -B + D ) / A ); 
  float t1 = ( -B - D ) / A; 
        float t2 = ( -B + D ) / A; 
        if(t1 < 0 && t2 < 0) 
   return false; 
         
  if(min(t1, t2) < 0) 
  { 
            time = max(t1,t2); 
            return true; 
        } 
  time = min(t1, t2); 
        return true; 
 } 
 return false; 
} 

bool IntersectTriangle(SRay ray, vec3 v1, vec3 v2, vec3 v3, out float time) 
{ 
    time = -1; 
    vec3 A = v2 - v1; 
    vec3 B = v3 - v1; 
    vec3 N = cross(A, B); 
    
    float NdotRayDirection = dot(N, ray.Direction); 
    if (abs(NdotRayDirection) < 0.001)  
        return false; 
    
    float d = dot(N, v1); 
    float t = -(dot(N, ray.Origin) - d) / NdotRayDirection; 
    if (t < 0)  
        return false; 
    
    vec3 P = ray.Origin + t * ray.Direction; 
    vec3 C; 
    
    vec3 edge1 = v2 - v1; 
    vec3 VP1 = P - v1; 
    C = cross(edge1, VP1); 
    if (dot(N, C) < 0) 
        return false; 
    
    vec3 edge2 = v3 - v2; 
    vec3 VP2 = P - v2; 
    C = cross(edge2, VP2); 
    if (dot(N, C) < 0)  
        return false; 
    
    vec3 edge3 = v1 - v3; 
    vec3 VP3 = P - v3; 
    C = cross(edge3, VP3); 
    if (dot(N, C) < 0)  
        return false; 
    
    time = t; 
    return true; 
}

bool Raytrace ( SRay ray, SSphere spheres[2], STriangle triangles[22], SMaterial 
materials[9], float start, float final, inout SIntersection intersect ) 
{ 
bool result = false; 
float test = start; 
intersect.Time = final; 
//calculate intersect with spheres 
for(int i = 0; i < 2; i++) 
{ 
    SSphere sphere = spheres[i]; 
    if( IntersectSphere (sphere, ray, start, final, test ) && test < intersect.Time ) 
    { 
        intersect.Time = test; 
        intersect.Point = ray.Origin + ray.Direction * test;
       
        intersect.Normal = normalize(intersect.Point - spheres[i].Center);
        intersect.Color = materials[sphere.MaterialIdx].Color;
        intersect.LightCoeffs =   materials[sphere.MaterialIdx].LightCoeffs; 
        intersect.ReflectionCoef = materials[sphere.MaterialIdx].ReflectionCoef; 
        intersect.RefractionCoef = materials[sphere.MaterialIdx].RefractionCoef; 
        intersect.MaterialType = materials[sphere.MaterialIdx].MaterialType; 
        result = true; 
    } 
} 
//calculate intersect with triangles 
for(int i = 0; i < 22; i++) 
{ 
    STriangle triangle = triangles[i]; 
 
    if(IntersectTriangle(ray, triangle.v1, triangle.v2, triangle.v3, test)  
       && test < intersect.Time) 
 { 
        intersect.Time = test; 
        intersect.Point = ray.Origin + ray.Direction * test; 
        intersect.Normal =  
             normalize(cross(triangle.v1 - triangle.v2, triangle.v3 - triangle.v2)); 
       intersect.Color = materials[triangle.MaterialIdx].Color;
        intersect.LightCoeffs =   materials[triangle.MaterialIdx].LightCoeffs; 
        intersect.ReflectionCoef = materials[triangle.MaterialIdx].ReflectionCoef; 
        intersect.RefractionCoef = materials[triangle.MaterialIdx].RefractionCoef; 
        intersect.MaterialType = materials[triangle.MaterialIdx].MaterialType; 
        result = true; 
    } 
 
} 
return result; 
} 
 
void initializeDefaultLightMaterials(out SLight light, out SMaterial materials[9]) 
{ 
    //** LIGHT **// 
    light.Position = vec3(0.0, 2.0, -4.0f); 
 
    /** MATERIALS **/ 
    vec4 lightCoefs = vec4(0.4,0.9,0.0,512.0); 
    materials[0].Color = vec3(RW, RW, 1.0); 
    materials[0].LightCoeffs = vec4(lightCoefs); 
    materials[0].ReflectionCoef = 0.5; 
    materials[0].RefractionCoef = 1.0; 
    materials[0].MaterialType = DIFFUSE;  
 
    materials[1].Color = vec3(0.9, 0.2, 1.0); 
    materials[1].LightCoeffs = vec4(lightCoefs); 
    materials[1].ReflectionCoef = 0.5; 
    materials[1].RefractionCoef = 1.0; 
    materials[1].MaterialType = DIFFUSE;  

    materials[2].Color = vec3(1.0, 0.0, 0.0);
    materials[2].LightCoeffs = lightCoefs;
    materials[2].ReflectionCoef = 0.3;
    materials[2].RefractionCoef = 1.0;
    materials[2].MaterialType = DIFFUSE;
    
    materials[3].Color = vec3(1.0, 1.0, 1.0);
    materials[3].LightCoeffs = lightCoefs;
    materials[3].ReflectionCoef = 0.8;  
    materials[3].RefractionCoef = 1.0;
    materials[3].MaterialType = DIFFUSE;

    materials[4].Color = vec3(1.0, 1.0, 1.0);
    materials[4].LightCoeffs = lightCoefs;
    materials[4].ReflectionCoef = 0.8; 
    materials[4].RefractionCoef = 1.0;
    materials[4].MaterialType = MIRROR_REFLECTION;

    materials[5].Color = vec3(1.0, 1.0, 1.0);
    materials[5].LightCoeffs = lightCoefs;
    materials[5].ReflectionCoef = 0.15; 
    materials[5].RefractionCoef = 1.5;
    materials[5].MaterialType = REFRACTION;

    materials[6].Color = vec3(1.0, 0.69, 0.8); 
    materials[6].LightCoeffs = vec4(0.5, 0.8, 0.3, 32.0); 
    materials[6].ReflectionCoef = 0.2; 
    materials[6].RefractionCoef = 1.0; 
    materials[6].MaterialType = DIFFUSE;
}

bool Refract(vec3 incident, vec3 normal, float ior, out vec3 refracted) {
    float eta = 1.0 / ior;
    float NdotI = dot(normal, incident);
    float k = 1.0 - eta * eta * (1.0 - NdotI * NdotI);
    
    if (k < 0.0) {
        return false; 
    }
    
    refracted = eta * incident - (eta * NdotI + sqrt(k)) * normal;
    return true;
}

float Shadow(SLight currLight, SIntersection intersect) {
    float shadowing = 1.0;
    vec3 lightDir = currLight.Position - intersect.Point;
    float distanceToLight = length(lightDir);
    lightDir = normalize(lightDir);
    
    vec3 shadowRayOrigin = intersect.Point + intersect.Normal * EPSILON * 2.0;
    vec3 direction = normalize(currLight.Position - intersect.Point);
    SRay shadowRay = SRay(shadowRayOrigin, lightDir);
    SIntersection shadowIntersect;
    shadowIntersect.Time = BIG;
    
    if (Raytrace(shadowRay, spheres, triangles, materials, 0.0, distanceToLight, shadowIntersect)) {
       
        if (shadowIntersect.Time < distanceToLight - EPSILON) {
            shadowing = 0.0;
        }
    }
    return shadowing;
}


vec3 Phong ( SIntersection intersect, SLight currLight, float shadow ) 
{ 
    vec3 light = normalize ( currLight.Position - intersect.Point ); 
    float diffuse = max(dot(light, intersect.Normal), 0.0); 
    vec3 view = normalize(uCamera.Position - intersect.Point); 
    vec3 reflected= reflect( -view, intersect.Normal ); 
    float specular = pow(max(dot(reflected, light), 0.0), intersect.LightCoeffs.w); 
    return intersect.LightCoeffs.x * intersect.Color + 
    intersect.LightCoeffs.y * diffuse * intersect.Color * shadow + 
    intersect.LightCoeffs.z * specular * 3; 
}


const int STACK_SIZE = 42;
STracingRay rayStack[STACK_SIZE];
int stackTop = -1;

void pushRay(STracingRay ray) {
    if (stackTop < STACK_SIZE - 1) {
        stackTop++;
        rayStack[stackTop] = ray;
    }
}

STracingRay popRay() {
    STracingRay ray = rayStack[stackTop];
    stackTop--;
    return ray;
}

bool isEmpty() {
    return stackTop < 0;
}

void main()
{
    float start = 0; 
    float final = BIG; 
    initializeDefaultLightMaterials(light, materials);
    initializeDefaultScene(triangles, spheres);
    uCamera = initializeDefaultCamera(); 
    
    vec3 resultColor = vec3(0,0,0); 
    
    SRay primaryRay = GenerateRay(uCamera);
    STracingRay trRay = STracingRay(primaryRay, 1.0, 0);
    pushRay(trRay);
    
    while (!isEmpty()) {
        STracingRay trRay = popRay();
        SRay ray = trRay.ray;
        
        SIntersection intersect;
        intersect.Time = BIG;
        
        if (Raytrace(ray, spheres, triangles, materials, start, final, intersect)) {
            switch(intersect.MaterialType) {
                case DIFFUSE: {
                    float shadowing = Shadow(light, intersect);
                    resultColor += trRay.contribution * Phong(intersect, light, shadowing);
                    break;
                }
                case MIRROR_REFLECTION: {
                    if (intersect.ReflectionCoef < 1.0) {
                        float contribution = trRay.contribution * (1.0 - intersect.ReflectionCoef);
                        float shadowing = Shadow(light, intersect);
                        resultColor += contribution * Phong(intersect, light, shadowing);
                    }
                    
                    if (trRay.depth < MAX_DEPTH) {
                        vec3 reflectDirection = reflect(ray.Direction, intersect.Normal);
                        float contribution = trRay.contribution * intersect.ReflectionCoef;
                        STracingRay reflectRay = STracingRay(
                            SRay(intersect.Point + reflectDirection * EPSILON, reflectDirection),
                            contribution,
                            trRay.depth + 1);
                        pushRay(reflectRay);
                    }
                    break;
                }
                case REFRACTION: {
                  
                    float fresnel = pow(1.0 - max(dot(-ray.Direction, intersect.Normal), 0.0), 5.0);
                    fresnel = mix(0.1, 1.0, fresnel); 
                    float shadowing = Shadow(light, intersect);
                    resultColor += trRay.contribution * 0.03 * Phong(intersect, light, shadowing);
                    if (trRay.depth < MAX_DEPTH) {
                       
                        vec3 refractDir;
                        float ior = intersect.RefractionCoef;
                        vec3 normal = intersect.Normal;
                        
                      
                        if (dot(ray.Direction, normal) > 0.0) {
                            normal = -normal;
                            ior = 1.0 / ior;
                        }
                        
                        if (Refract(ray.Direction, normal, ior, refractDir)) {
                            STracingRay refractRay = STracingRay(
                                SRay(intersect.Point + refractDir * EPSILON, refractDir),
                                trRay.contribution * (1.0 - fresnel),
                                trRay.depth + 1
                            );
                            pushRay(refractRay);
                        }
                        
                      
                        vec3 reflectDir = reflect(ray.Direction, intersect.Normal);
                        STracingRay reflectRay = STracingRay(
                            SRay(intersect.Point + reflectDir * EPSILON, reflectDir),
                            trRay.contribution * fresnel,
                            trRay.depth + 1
                        );
                        pushRay(reflectRay);
                    }
                    break;
                }
            }
        }
    }
    
    FragColor = vec4(resultColor, 1.0);
}