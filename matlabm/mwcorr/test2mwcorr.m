tic,
d=sgdiff(test,21,2);
xc=smooth(d,5);
xc=[xc(132:313,:);xc(469:676,:)];% ;��ѡ������4003.5-4701.6cm-1������132-313����5303.3-6101.7cm-1������469-676��
[xc,mx,sd]=normpathlength(xc);
[minSQ,TQ,ad]=IdentifyPredictor(xx,xc,11,5);
TQ,ad,
toc,

%-----PCA--------------
tic,
d=sgdiff(test,21,2);
xc=smooth(d,5);
xc=[xc(132:313,:);xc(469:676,:)];% ;��ѡ������4003.5-4701.6cm-1������132-313����5303.3-6101.7cm-1������469-676��
[xc,mx,sd]=normpathlength(xc);
[m,n]=size(t);
[tun] = ripppcapred(xc, p, w );
tt=t-ones(m,1)*tun;
sunt=sum(abs(tt'));
 [r2,index]=sort(sunt);
 nxx=xx(:,index(1:20));
 [minSQ,TQ,ad]=IdentifyPredictor(nxx,xc,11,5);
 ad2=(index(ad));
 TQ,ad2,
 toc,