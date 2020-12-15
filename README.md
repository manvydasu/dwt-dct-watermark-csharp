# DWT DCT Watermark c#
Implementation of DWT-DCT image watermarking algorithm using c#

# Theory and implementation:
The implementation is based on "Combined DWT-DCT digital image watermarking" research paper
https://www.researchgate.net/publication/26621646_Combined_DWT-DCT_digital_image_watermarking

  Image watermarking Algorithm:
* Apply DWT on target image to split it into 4 sub-bands.
* Divide HL1 sub-band from previous step into 4x4 blocks.
* Take grey-scale watermark image and convert it into vector of 0 and 1 (white and black).
* Generate two uncorelated PN sequences.
* Embed each pixel of watermark image in to mid-band coefficients of DCT blocks using PN sequences. 
* Apply inverse DCT to each 4x4 block.
* Apply inverse DWT to produce watermarked iamge.

Watermark retrieval algorithm:
* Apply DWT on target image to split it into 4 sub-bands.
* Divide HL1 sub-band from previous step into 4x4 blocks.
* Regenerate PN sequences from watermark embedidng step using same seed.
* For each DCT block, find correlation between mid-band coefficients and PN sequences and determine if extracted watermark bit should be 0 or 1.
* Reconstruct the watermark.
 
Steps are described with a little more details in the paper, but those above should be enough to understand the code if you are familiar with image watermarking.


# Results:

Extracing from uncompressed image:
![extracted_uncompressed](https://github.com/manvydasu/dwt-dct-watermark-csharp/tree/main/results/extracted_uncompressed.jpg)

Extracting from 75% jpeg compressed image:
![extracted_after_jpeg_75_compression](https://github.com/manvydasu/dwt-dct-watermark-csharp/tree/main/results/extracted_after_jpeg_75_compression.jpg)

Seems like the result is not very robust to jpeg compression.
In addition, the original image gets quite visibly distorted if strength of embedding is increased.
