```text
$ ./snapselect -h
Description:
  SnapSelect helps users select which files to include in snapshots for backups.
  Use one of these commands: listforget, lf

Usage:
  snapselect [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  lf, listforget  The command `listforget` shows the JSON result from `restic forget` in a table like `restic snapshots`.
                  Usage: `restic forget --dry-run --json POLICIES | snapselect lf`
```
```text
$ ./snapselect lf -h
Description:
  The command `listforget` shows the JSON result from `restic forget` in a table like `restic snapshots`.
  Usage: `restic forget --dry-run --json POLICIES | snapselect lf`

Usage:
  snapselect listforget [options]

Options:
  -g, --group     This option is not yet implememted:
                  Show one table per group, instead of a column for groups.
  -?, -h, --help  Show help and usage information
```
```text
$ restic forget --dry-run --json \
  --keep-within-monthly 10y \
  --keep-within-weekly 63d \
  --keep-within-daily 14d \
  --keep-within-hourly 48h | ./snapselect lf\
∙
Grouping into 3 groups:
G  Host        Path
--------------------------
A  alpha       /
B  Omicron     /
C  alpha       /media/hdd

Applying 4 policies:
1  hourly within 48h
2  daily within 14d
3  weekly within 63d
4  monthly within 10y

Keeping 21 snapshots:
ID        Original  Date       Time      G  Result  Reasons
-----------------------------------------------------------
1522a12f  59cb8d05  2023-02-24 15:39:11  B  keep    1 2 3 4
3b46277b  5abf0643  2023-03-26 04:18:42  C  keep    1 2 3 4
b5020f8c  b548a9b3  2023-04-02 03:38:16  A  keep    · · 3 ·
46553cce  6a67b681  2023-04-03 12:56:12  A  remove
17c3a34d  b8d210e5  2023-04-08 20:20:21  A  remove
6db99f4c  214bf830  2023-04-09 16:38:32  A  keep    · · 3 ·
2d5302bb  1f1159c2  2023-04-10 23:45:16  A  remove
690a0f2c  0edeca6d  2023-04-16 00:40:31  A  remove
7d37758f  1c41e2c8  2023-04-16 19:17:36  A  keep    · · 3 ·
3e885a9d  6c0b9700  2023-04-17 21:57:35  A  remove
68a3c02d  3c1f6dbe  2023-04-23 04:01:37  A  remove
0c6f60ea  436f6f2d  2023-04-23 22:32:38  A  keep    · · 3 ·
5da96c6a  f04c5246  2023-04-24 23:45:35  A  remove
a8d73492            2023-04-27 23:31:38  A  remove
34c26314            2023-04-29 22:44:46  A  keep    · 2 · ·
9c6c1e53            2023-04-30 23:57:32  A  keep    · 2 3 4
75b17ea9            2023-05-01 23:52:29  A  keep    · 2 · ·
c7cc71ce            2023-05-02 22:33:22  A  keep    · 2 · ·
85556c85            2023-05-03 12:57:57  A  remove
760ea8c5            2023-05-03 23:12:56  A  keep    · 2 · ·
3bce5b69            2023-05-04 00:13:06  A  remove
6e88cc9a            2023-05-04 14:41:55  A  keep    · 2 · ·
037c1731            2023-05-05 08:55:56  A  remove
67cf2195            2023-05-05 20:40:01  A  remove
6d04d3a5            2023-05-05 23:20:54  A  keep    · 2 · ·
280edc54            2023-05-06 23:52:29  A  keep    · 2 · ·
f8da5b7c            2023-05-07 22:08:10  A  keep    · 2 3 ·
23548105            2023-05-08 00:08:37  A  remove
c8e09bb0            2023-05-08 01:55:13  A  remove
d3b60403            2023-05-08 23:13:28  A  keep    · 2 · ·
42fc3484            2023-05-09 23:59:05  A  keep    · 2 · ·
e18be1a0            2023-05-10 18:43:11  A  remove
7e12f878            2023-05-10 22:48:47  A  remove
0dde6ba3            2023-05-10 23:02:08  A  keep    · 2 · ·
c21f2d8a            2023-05-11 23:01:54  A  keep    1 2 · ·
eecbcf84            2023-05-12 14:53:37  A  keep    1 · · ·
9a50b4d6            2023-05-12 23:05:51  A  keep    1 2 3 4
```