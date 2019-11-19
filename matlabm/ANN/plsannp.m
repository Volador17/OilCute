clear all
load d:\valid_x50.txt
load d:\valid_y50.txt
load d:\nn

xx=valid_x50';
yy=valid_y50;

f1='tansig';
f2='purelin';

for j=1:d
    w10(:,:)=w1(j,:,:);
    b10=b1(:,j);
    w20=w2(j,:);
    b20=b2(j);
yreg(j,:)=simuff(xx,w10,b10,f1,w20,b20,f2)+my;
end

yp=mean(yreg)';

z=[yp yy yy-yp]