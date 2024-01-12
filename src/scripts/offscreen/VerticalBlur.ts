import * as THREE from 'three'
import { Canvas } from '../Canvas'
import { Offscreen } from './Offscreen'
import vertexShader from '../shader/screen.vs'
import fragmentShader from '../shader/verticalBlur.fs'

export class VerticalBlur extends Offscreen {
  constructor(canvas: Canvas, source: THREE.Texture) {
    const material = new THREE.RawShaderMaterial({
      uniforms: {
        tDiffuse: { value: source },
      },
      vertexShader,
      fragmentShader,
    })

    super(canvas, material)
  }
}
