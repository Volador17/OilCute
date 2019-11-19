% load 两条重复光谱
x=[new05_8cm_003_c1_03(:,2),new05_8cm_003_c1_04(:,2)];
d=sgdiff(x,21,2);
xc=smooth(d,5);

x=[xc(132:313,:);xc(469:676,:)]% ;先选则区间4003.5-4701.6cm-1（点数132-313）和5303.3-6101.7cm-1（点数469-676）
[xx,mx,sd]=normpathlength(x)
[TQ,SQ]=mwcorr(xx(:,1),xx(:,2),11);
    