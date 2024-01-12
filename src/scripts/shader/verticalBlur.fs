precision mediump float;

uniform sampler2D tDiffuse;

varying vec2 vUv;

#include './modules/gaussianBlur.glsl'

void main() {
  float dist = distance(vUv, vec2(0.5));
  dist = smoothstep(0.3, 0.6, dist);

  vec4 color = gaussianBlur(tDiffuse, vUv, vec2(0.0, 0.002) * dist);
  gl_FragColor = vec4(color.rgb, 1.0);
}