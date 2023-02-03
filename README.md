# KRand

I wrote this .NET library since nobody else seems to want inclusive max values in their randomizers for some reason.
Why do you guys have to be so weird?

Anyway I'm only making it a library since I've been using a version of this in pretty much everything I do, it has no dependencies, and it's lightning fast, so I might as well make it public.

It's like, really good.
It's based on the `Xoshiro256**` algorithm, which is the go-to in this time period I think.
You can supply your own seeds or not, the outputted values are evenly distributed, and it returns Boolean/8bit/16bit/32bit/64bit/Single/Double.
AND MOST IMPORTANTLY, THE MAXIMUM VALUES OF EACH RANGE ARE INCLUSIVE.

```cs
sbyte val = _rand.NextSByte(); // [sbyte.MinValue, sbyte.MaxValue]
short val = _rand.NextUInt16(-14, 23); // [-14, 23]
uint val = _rand.NextUInt32(); // [uint.MinValue, uint.MaxValue]
long val = _rand.NextUInt64(-5_000_000_000, 5_000_000_000); // [-5,000,000,000, 5,000,000,000]
float val = _rand.NextSingle(); // [0, 1]
double val = _rand.NextDouble(-500.123, 500.47); // [-500.123, 500.47]
bool val = _rand.NextBoolean(); // 1/2 (50%) True
bool val = _rand.NextBoolean(3, 10); // 3/10 (30%) True
bool val = _rand.NextBoolean(8, 447); // 8/447 (~1.78970917225951%) True
```

You can inherit from the `KRand` class too and add other stuff.
It has `Vector3` which just grabs 3 random floats, since it's very common to need 3 (color, position, etc.), so you can just add so much randomization like that but I kept Vector3 for everyone.

If you want to really see how evenly distributed it is, run the included tests.
I spent a lot of time and research to make the tests actually accurate with their reporting.

...Okay fine, of course I'll show you:

----
### Boolean: 50/50 chance | 1,000,000,000 iterations
```
False | 500,019,171 (50%)
 True | 499,980,829 (50%)
```
### Boolean: 30/70 chance | 1,000,000,000 iterations
```
False | 149,998,378 (30%)
 True | 350,001,622 (70%)
```
### Double: 50,000,000 iterations
```
Bucket 0 | [0.0, 0.1) | R AVG = 0.05 ||| 4,998,127 (10.00%) | AVG = 0.05000
Bucket 1 | [0.1, 0.2) | R AVG = 0.15 ||| 5,001,622 (10.00%) | AVG = 0.14999
Bucket 2 | [0.2, 0.3) | R AVG = 0.25 ||| 4,999,887 (10.00%) | AVG = 0.25000
Bucket 3 | [0.3, 0.4) | R AVG = 0.35 ||| 5,000,189 (10.00%) | AVG = 0.34998
Bucket 4 | [0.4, 0.5) | R AVG = 0.45 ||| 4,998,803 (10.00%) | AVG = 0.44998
Bucket 5 | [0.5, 0.6) | R AVG = 0.55 ||| 4,999,578 (10.00%) | AVG = 0.55002
Bucket 6 | [0.6, 0.7) | R AVG = 0.65 ||| 4,999,663 (10.00%) | AVG = 0.65001
Bucket 7 | [0.7, 0.8) | R AVG = 0.75 ||| 4,996,720 ( 9.99%) | AVG = 0.74999
Bucket 8 | [0.8, 0.9) | R AVG = 0.85 ||| 5,004,466 (10.01%) | AVG = 0.84998
Bucket 9 | [0.9, 1.0] | R AVG = 0.95 ||| 5,000,945 (10.00%) | AVG = 0.95001
```
### SByte: [-128, 127] range | 100,000,000 iterations
```
Bucket  0 | [-128.0000, -112.0625) | R AVG = -120.03125 ||| 6,250,533 (6.25%) | SUM = -753,187,785 | AVG = -120.49977
Bucket  1 | [-112.0625, - 96.1250) | R AVG = -104.09375 ||| 6,248,253 (6.25%) | SUM = -652,951,379 | AVG = -104.50143
Bucket  2 | [- 96.1250, - 80.1875) | R AVG = - 88.15625 ||| 6,249,423 (6.25%) | SUM = -553,068,401 | AVG = - 88.49911
Bucket  3 | [- 80.1875, - 64.2500) | R AVG = - 72.21875 ||| 6,248,881 (6.25%) | SUM = -453,024,650 | AVG = - 72.49692
Bucket  4 | [- 64.2500, - 48.3125) | R AVG = - 56.28125 ||| 6,251,631 (6.25%) | SUM = -353,232,661 | AVG = - 56.50248
Bucket  5 | [- 48.3125, - 32.3750) | R AVG = - 40.34375 ||| 6,251,325 (6.25%) | SUM = -253,155,494 | AVG = - 40.49629
Bucket  6 | [- 32.3750, - 16.4375) | R AVG = - 24.40625 ||| 6,248,317 (6.25%) | SUM = -153,076,669 | AVG = - 24.49886
Bucket  7 | [- 16.4375, -  0.5000) | R AVG = -  8.46875 ||| 6,248,556 (6.25%) | SUM = - 53,100,106 | AVG = -  8.49798
Bucket  8 | [-  0.5000,   15.4375) | R AVG =    7.46875 ||| 6,248,073 (6.25%) | SUM =   46,870,263 | AVG =    7.50155
Bucket  9 | [  15.4375,   31.3750) | R AVG =   23.40625 ||| 6,249,590 (6.25%) | SUM =  146,859,860 | AVG =   23.49912
Bucket 10 | [  31.3750,   47.3125) | R AVG =   39.34375 ||| 6,251,849 (6.25%) | SUM =  246,980,216 | AVG =   39.50515
Bucket 11 | [  47.3125,   63.2500) | R AVG =   55.28125 ||| 6,251,314 (6.25%) | SUM =  346,942,939 | AVG =   55.49920
Bucket 12 | [  63.2500,   79.1875) | R AVG =   71.21875 ||| 6,253,023 (6.25%) | SUM =  447,103,399 | AVG =   71.50196
Bucket 13 | [  79.1875,   95.1250) | R AVG =   87.15625 ||| 6,248,376 (6.25%) | SUM =  546,738,584 | AVG =   87.50091
Bucket 14 | [  95.1250,  111.0625) | R AVG =  103.09375 ||| 6,250,050 (6.25%) | SUM =  646,888,891 | AVG =  103.50139
Bucket 15 | [ 111.0625,  127.0000] | R AVG =  119.03125 ||| 6,250,806 (6.25%) | SUM =  746,964,105 | AVG =  119.49885
```
### UInt64: [1,000,000,000, 18,446,244,013,709,451,615] range | 50,000,000 iterations
```
Bucket 0 | [             1,000,000,000.0,  1,844,624,402,270,945,161.5) | R AVG =    922,312,201,635,472,580.75 ||| 5,000,435 (10.00%) | AVG =    922,071,315,607,355,502.93769
Bucket 1 | [ 1,844,624,402,270,945,161.5,  3,689,248,803,541,890,323.0) | R AVG =  2,766,936,602,906,417,742.25 ||| 5,002,258 (10.00%) | AVG =  2,766,803,572,496,434,941.79732
Bucket 2 | [ 3,689,248,803,541,890,323.0,  5,533,873,204,812,835,484.5) | R AVG =  4,611,561,004,177,362,903.75 ||| 4,995,752 ( 9.99%) | AVG =  4,611,651,192,054,776,904.48355
Bucket 3 | [ 5,533,873,204,812,835,484.5,  7,378,497,606,083,780,646.0) | R AVG =  6,456,185,405,448,308,065.25 ||| 5,002,502 (10.01%) | AVG =  6,456,524,748,594,091,406.81485
Bucket 4 | [ 7,378,497,606,083,780,646.0,  9,223,122,007,354,725,807.5) | R AVG =  8,300,809,806,719,253,226.75 ||| 4,998,819 (10.00%) | AVG =  8,300,821,453,186,655,145.21449
Bucket 5 | [ 9,223,122,007,354,725,807.5, 11,067,746,408,625,670,969.0) | R AVG = 10,145,434,207,990,198,388.25 ||| 5,003,399 (10.01%) | AVG = 10,145,752,869,454,861,042.56255
Bucket 6 | [11,067,746,408,625,670,969.0, 12,912,370,809,896,616,130.5) | R AVG = 11,990,058,609,261,143,549.75 ||| 4,999,109 (10.00%) | AVG = 11,990,129,384,677,121,044.73323
Bucket 7 | [12,912,370,809,896,616,130.5, 14,756,995,211,167,561,292.0) | R AVG = 13,834,683,010,532,088,711.25 ||| 5,000,993 (10.00%) | AVG = 13,834,487,856,311,467,824.43183
Bucket 8 | [14,756,995,211,167,561,292.0, 16,601,619,612,438,506,453.5) | R AVG = 15,679,307,411,803,033,872.75 ||| 4,998,900 (10.00%) | AVG = 15,679,228,538,423,324,175.10517
Bucket 9 | [16,601,619,612,438,506,453.5, 18,446,244,013,709,451,615.0] | R AVG = 17,523,931,813,073,979,034.25 ||| 4,997,833 (10.00%) | AVG = 17,523,721,492,624,964,796.14862
```

----
## KRandTesting Uses:
* [xUnit.net](https://github.com/xunit/xunit)