% 标准化程序，用于校正集，如果用于校正集外样品，参见atscalep；
% input
% x:吸光度矩阵；
% 
% output
% xc:处理后的吸光度矩阵；
% mx:平均光谱；
% sx:标准偏差光谱；

function [xc,mx,sx] = atscale(x);
	[m,n]=size(x);
	mx=mean(x,2);
	sx=std(x,[],2);
	II=find(sx==0);%寻找sx为0的项,在计算xc时不予计算，而设为0
	xc=zeros(m,n);
	III=1:m;
	III(II)=[];
	xc(III,:)=(x(III,:)-mx(III,:)*ones(1,n))./(sx(III,:)*ones(1,n));
end