clear all
load d:\model_x100.txt
load d:\model_y100.txt
load d:\test_x50.txt
load d:\test_y50.txt

x=model_x100';
y=model_y100';
xx=test_x50';
yy=test_y50';
trainf='trainlm';
hm=5;
f1='tansig';
f2='purelin';
tn=200;
traino=0.0001;
d=100;
k=60;

[m,n]=size(x);
my=mean(y);
by=y-my*ones(1,n);
byy=yy-my*ones(1,length(yy));

[w1,b1,w2,b2]=bann2(x,by,xx,byy,trainf,hm,f1,f2,tn,traino,d,k);

for j=1:d
    w10(:,:)=w1(j,:,:);
    b10=b1(:,j);
    w20=w2(j,:);
    b20=b2(j);
yreg(j,:)=simuff(xx,w10,b10,f1,w20,b20,f2)+my;
end

yp=mean(yreg)';
z=[yy' yp yy'-yp]

save d:\nn w1 b1 w2 b2 my d