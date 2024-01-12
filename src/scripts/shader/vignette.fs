precision mediump float;

uniform sampler2D tDiffuse;
uniform vec2 uResolution;

varying vec2 vUv;

void main() {
  vec3 color = texture2D(tDiffuse, vUv).rgb;
  vec2 aspect = vec2(uResolution.x / uResolution.y, 1.0);

  float dist = distance(vUv * aspect, vec2(0.5) * aspect);
  dist = 1.0 - smoothstep(0.3 * aspect.x, 0.5 * aspect.x, dist);
  color *= dist;

  gl_FragColor = vec4(color, 1.0);
}